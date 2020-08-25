﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GBX.NET
{
    public class GameBoxReader : BinaryReader
    {
        public ILookbackable Lookbackable { get; }

        public GameBoxReader(Stream input) : base(input, Encoding.UTF8, true)
        {

        }

        public GameBoxReader(Stream input, ILookbackable lookbackable) : this(input)
        {
            Lookbackable = lookbackable;
        }

        public string ReadString(StringLengthPrefix readPrefix)
        {
            int length;
            if (readPrefix == StringLengthPrefix.Byte)
                length = ReadByte();
            else if (readPrefix == StringLengthPrefix.Int32)
                length = ReadInt32();
            else
                throw new Exception("Can't read string without knowing its length.");
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads a string from the current stream. The string is prefixed with the length, encoded as <see cref="int"/>.
        /// </summary>
        /// <returns></returns>
        public override string ReadString()
        {
            return ReadString(StringLengthPrefix.Int32);
        }

        public string ReadString(int length)
        {
            return new string(ReadChars(length));
        }

        /// <summary>
        /// Reads the next <see cref="int"/> from the current stream, casts it as <see cref="bool"/> and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <returns></returns>
        public override bool ReadBoolean()
        {
            return Convert.ToBoolean(ReadInt32());
        }

        public bool ReadBoolean(bool asByte)
        {
            if (asByte) return base.ReadBoolean();
            else return ReadBoolean();
        }

        public LookbackString ReadLookbackString(ILookbackable lookbackable)
        {
            if (!lookbackable.LookbackVersion.HasValue)
                lookbackable.LookbackVersion = ReadInt32();

            var index = ReadUInt32();

            if ((index & 0x3FFF) == 0 && (index >> 30 == 1 || index >> 30 == 2))
            {
                var str = ReadString();
                lookbackable.LookbackStrings.Add(str);
                return new LookbackString(str, lookbackable);
            }
            else if ((index & 0x3FFF) == 0x3FFF)
            {
                return (index >> 30) switch
                {
                    2 => new LookbackString("Unassigned", lookbackable),
                    3 => new LookbackString("", lookbackable),
                    _ => throw new Exception(),
                };
            }
            else if (index >> 30 == 0)
            {
                if (LookbackString.CollectionIDs.TryGetValue((int)index, out string val))
                    return new LookbackString(val, lookbackable);
                else
                    return new LookbackString("???", lookbackable);
            }
            else if (lookbackable.LookbackStrings.Count > (index & 0x3FFF) - 1)
                return new LookbackString(lookbackable.LookbackStrings[(int)(index & 0x3FFF) - 1], lookbackable);
            else
                return new LookbackString("", lookbackable);
        }

        public LookbackString ReadLookbackString()
        {
            return ReadLookbackString(Lookbackable);
        }

        public Meta ReadMeta(ILookbackable lookbackable)
        {
            var id = ReadLookbackString(lookbackable);
            var collection = ReadLookbackString(lookbackable);
            var author = ReadLookbackString(lookbackable);

            return new Meta(id, collection, author);
        }

        public Meta ReadMeta()
        {
            return ReadMeta(Lookbackable);
        }

        public Node ReadNodeRef(IGameBoxBody body)
        {
            var index = ReadInt32() - 1;

            if (index >= 0 && body.AuxilaryNodes.Count <= index)
            {
                var classID = ReadUInt32();
                Debug.WriteLine("Node ref class: " + classID.ToString("x8"));
                body.AuxilaryNodes.Add(Node.Parse(body, classID, this));
            }

            return body.AuxilaryNodes.ElementAtOrDefault(index);
        }

        public Node ReadNodeRef()
        {
            return ReadNodeRef((IGameBoxBody)Lookbackable);
        }

        public T ReadNodeRef<T>(bool hasInheritance, IGameBoxBody body) where T : Node
        {
            var index = ReadInt32() - 1; // GBX seems to start the index at 1

            if (index >= 0 && body.AuxilaryNodes.ElementAtOrDefault(index) == null) // If index is 0 or bigger and the node wasn't read yet
            {
                var classID = ReadUInt32();
                Debug.WriteLine("Node ref class: " + classID.ToString("x8"));

                Node node;
                if (hasInheritance)
                    node = Node.Parse(body, classID, this);
                else
                    node = Node.Parse<T>(body, this);

                if (index >= body.AuxilaryNodes.Count)
                    body.AuxilaryNodes.Add(node);
                else
                    body.AuxilaryNodes.Insert(index, node);
            }

            if (index < 0) // If aux node index is below 0 then there's not much to solve
                return null;
            var nod = (T)body.AuxilaryNodes.ElementAtOrDefault(index); // Tries to get the available node from index
            if (nod == null) // But sometimes it indexes the node reference that is further in the expected indexes
                return (T)body.AuxilaryNodes.Last(); // So it grabs the last one instead, needs to be further tested
            else // If the node is presented at the index, then it's simple
                return nod;
        }

        public T ReadNodeRef<T>(bool hasInheritance) where T : Node
        {
            return ReadNodeRef<T>(hasInheritance, (IGameBoxBody)Lookbackable);
        }

        public T ReadNodeRef<T>() where T : Node
        {
            return ReadNodeRef<T>(false);
        }

        public FileRef ReadFileRef()
        {
            var version = ReadByte();

            byte[] checksum = null;
            string locatorUrl = "";

            if (version >= 3)
                checksum = ReadBytes(32);

            var filePath = ReadString();

            if ((filePath.Length > 0 && version >= 1) || version >= 3)
                locatorUrl = ReadString();

            return new FileRef(version, checksum, filePath, locatorUrl);
        }

        public T[] ReadArray<T>(int count)
        {
            var buffer = ReadBytes(count * Marshal.SizeOf(default(T)));
            var array = new T[count];
            Buffer.BlockCopy(buffer, 0, array, 0, buffer.Length);
            return array;
        }

        /// <summary>
        /// First reads an <see cref="int"/> representing the length, then does a for loop with this length, each element requiring to return an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="forLoop">Each element.</param>
        /// <returns>An array of <typeparamref name="T"/>.</returns>
        public T[] ReadArray<T>(Func<int, T> forLoop)
        {
            var length = ReadInt32();
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = forLoop.Invoke(i);

            return result;
        }

        public Vector2 ReadVec2()
        {
            var floats = ReadArray<float>(2);
            return new Vector2(floats[0], floats[1]);
        }

        public Vector3 ReadVec3()
        {
            var floats = ReadArray<float>(3);
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        public Int3 ReadInt3()
        {
            var ints = ReadArray<int>(3);
            return (ints[0], ints[1], ints[2]);
        }

        public Int2 ReadInt2()
        {
            var ints = ReadArray<int>(2);
            return (ints[0], ints[1]);
        }

        public Byte3 ReadByte3()
        {
            var bytes = ReadBytes(3);
            return (bytes[0], bytes[1], bytes[2]);
        }

        public uint PeekUInt32()
        {
            var result = ReadUInt32();
            BaseStream.Position -= sizeof(uint);
            return result;
        }
    }
}