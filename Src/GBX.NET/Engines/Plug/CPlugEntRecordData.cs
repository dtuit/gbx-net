﻿using System;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;

using System;
using System.IO;

namespace GBX.NET.Engines.Plug;

/// <summary>
/// Entity data in a timeline.
/// </summary>
/// <remarks>ID: 0x0911F000</remarks>
[Node(0x0911F000)]
public class CPlugEntRecordData : CMwNod
{
    private ObservableCollection<Sample>? samples;

    public byte[]? Data { get; set; }

    [NodeMember]
    [AppliedWithChunk<Chunk0911F000>]
    public ObservableCollection<Sample>? Samples
    {
        get
        {
            if (samples is null)
            {
                var chunkVersion = GetChunk<Chunk0911F000>()?.Version;

                if (chunkVersion.HasValue)
                {
                    samples = ReadSamples(chunkVersion.Value);
                }
            }

            return samples;
        }
    }

    internal CPlugEntRecordData()
    {
        
    }

    public void PrintByteArray(byte[] bytes)
    {
        var sb = new StringBuilder("new byte[] { ");
        foreach (var b in bytes)
        {
            sb.Append(b + ", ");
        }
        sb.Append("}");
        Console.WriteLine(sb.ToString());
    }

    public string ByteArrayToString(byte[] bytes)
    {
        var sb = new StringBuilder("");
        foreach (var b in bytes)
        {
            sb.Append(b + ", ");
        }
        return sb.ToString();
    }

    private ObservableCollection<Sample> ReadSamples(int version)
    {
        if (Data is null)
        {
            throw new Exception();
        }

        string filePath = "C:\\Users\\darren\\code\\gbx-net\\DebugGhosts\\recordbytes_v2_surfacetype_skid.csv";
        using StreamWriter file = new(filePath);

        //using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileAccess.Write)))
        //{
        //    writer.WriteLine("sep=,");
        //    writer.WriteLine("Hello, Goodbye");
        //}

        var samples = new ObservableCollection<Sample>();

        using var ms = new MemoryStream(Data);
        using var cs = new CompressedStream(ms, CompressionMode.Decompress);
        using var r = new GameBoxReader(cs);

        var u01 = r.ReadInt32();
        var ghostLength = r.ReadInt32(); // milliseconds
        var objects = r.ReadArray<object>(r =>
        {
            var nodeId = r.ReadUInt32();
            var nodeName = NodeManager.GetName(nodeId);

            var obj_u01 = r.ReadInt32();
            var obj_u02 = r.ReadInt32();
            var obj_u03 = r.ReadInt32();
            var mwbuffer = r.ReadInt32();
            var obj_u05 = r.ReadInt32();

            return new
            {
                nodeId,
                nodeName,
                obj_u01,
                obj_u02,
                obj_u03,
                mwbuffer,
                obj_u05
            };
        });

        if (version >= 2)
        {
            var objcts2 = r.ReadArray<object>(r =>
            {
                var u02 = r.ReadInt32();
                var u03 = r.ReadInt32();

                uint? clas = null;
                string? clasName = null;

                if (version >= 4)
                {
                    clas = r.ReadUInt32();
                    clasName = NodeManager.GetName(clas.Value);
                }

                return new
                {
                    u02,
                    u03,
                    clas,
                    clasName
                };
            });
            Console.WriteLine("");
        }

        var u04 = r.ReadByte();
        while (u04 != 0)
        {
            var bufferType = r.ReadInt32();
            var u06 = r.ReadInt32();
            var u07 = r.ReadInt32();
            var ghostLengthFinish = r.ReadInt32(); // ms

            if (version < 6)
            {
                // temp_79f24995b2b->field_0x28 = temp_79f24995b2b->field_0xc
            }
            else
            {
                var u08 = r.ReadInt32();
            }

            // Reads byte on every loop until the byte is 0, should be 1 otherwise
            for (byte x; (x = r.ReadByte()) != 0;)
            {
                var timestamp = r.ReadInt32();
                
                var buffer = r.ReadBytes(); // MwBuffer
                //Console.WriteLine();
                
                //Console.WriteLine($"{timestamp}");

                //Console.WriteLine(Encoding.Default.GetString(buffer));

                PrintByteArray(buffer);



                if (buffer.Length == 0)
                {
                    continue;
                }

                using var bufferMs = new MemoryStream(buffer);
                using var bufferR = new GameBoxReader(bufferMs);

                var sampleProgress = (int)bufferMs.Position;

                var sample = new Sample(buffer)
                {
                    BufferType = (byte)bufferType
                };

                switch (bufferType)
                {
                    case 0:
                        break;
                    case 2:
                        {
                            bufferMs.Position = 5;

                            var (position, rotation, speed, velocity) = bufferR.ReadTransform(); // Only position matches

                            sample.Timestamp = TimeInt32.FromMilliseconds(timestamp);
                            sample.Position = position;
                            sample.Rotation = rotation;
                            sample.Speed = speed * 3.6f;
                            sample.Velocity = velocity;

                            break;
                        }
                    case 4:
                    case 6:
                        {
                            file.WriteLine($"{timestamp},{buffer.Length},{ByteArrayToString(buffer)}");

                            bufferMs.Position = 5;
                            var rpmByte = bufferR.ReadByte();

                            bufferMs.Position = 14;
                            var steerByte = bufferR.ReadByte();
                            var steer = ((steerByte / 255f) - 0.5f) * 2;

                            bufferMs.Position = 91;
                            var gearByte = bufferR.ReadByte();
                            var gear = gearByte / 5f;

                            sample.Gear = gear;
                            sample.RPM = rpmByte;
                            sample.Steer = steer;

                            bufferMs.Position = 15;
                            var u15 = bufferR.ReadByte();

                            bufferMs.Position = 18;
                            var brakeByte = bufferR.ReadByte();
                            var brake = brakeByte / 255f;
                            var gas = u15 / 255f + brake;

                            sample.Brake = brake;
                            sample.Gas = gas;

                            bufferMs.Position = 47;

                            var (position, rotation, speed, velocity) = bufferR.ReadTransform();

                            sample.Timestamp = TimeInt32.FromMilliseconds(timestamp);
                            sample.Position = position;
                            sample.Rotation = rotation;
                            sample.Speed = speed * 3.6f;
                            sample.Velocity = velocity;

                            // ICE
                            bufferMs.Position = 81;
                            var FLiceByte = bufferR.ReadByte();
                            bufferMs.Position = 82;
                            var FRIceByte = bufferR.ReadByte();
                            bufferMs.Position = 83;
                            var BRIceByte = bufferR.ReadByte();
                            bufferMs.Position = 84;
                            var BLIceByte = bufferR.ReadByte();

                            sample.FLDirt = FLiceByte / 255f;
                            sample.FRDirt = FRIceByte / 255f;
                            sample.BRDirt = BRIceByte / 255f;
                            sample.BRDirt = BLIceByte / 255f;

                            // DIRT
                            bufferMs.Position = 93;
                            var FLDirtByte = bufferR.ReadByte();
                            bufferMs.Position = 95;
                            var FRDirtByte = bufferR.ReadByte();
                            bufferMs.Position = 97;
                            var BRDirtByte = bufferR.ReadByte();
                            bufferMs.Position = 99;
                            var BLDirtByte = bufferR.ReadByte();

                            sample.FLDirt = FLDirtByte / 255f;
                            sample.FRDirt = FRDirtByte / 255f;
                            sample.BRDirt = BRDirtByte / 255f;
                            sample.BRDirt = BLDirtByte / 255f;

                            // GroundContactMaterial
                            bufferMs.Position = 24;
                            var FLGroundContactMaterialByte = bufferR.ReadByte();
                            bufferMs.Position = 26;
                            var FRGroundContactMaterialByte = bufferR.ReadByte();
                            bufferMs.Position = 28;
                            var BLGroundContactMaterialByte = bufferR.ReadByte();
                            bufferMs.Position = 30;
                            var BRGroundContactMaterialByte = bufferR.ReadByte();

                            sample.FLGroundContactMaterial = (EPlugSurfaceMaterialId)FLGroundContactMaterialByte;
                            sample.FRGroundContactMaterial = (EPlugSurfaceMaterialId)FRGroundContactMaterialByte;
                            sample.BLGroundContactMaterial = (EPlugSurfaceMaterialId)BLGroundContactMaterialByte;
                            sample.BRGroundContactMaterial = (EPlugSurfaceMaterialId)BRGroundContactMaterialByte;

                            // Water
                            bufferMs.Position = 101;
                            var waterByte = bufferR.ReadByte();

                            sample.WetnessValue = waterByte / 255f;


                            // IsGroundContact, IsReactorGroundMode
                            bufferMs.Position = 89;
                            var groundModeByte = bufferR.ReadByte();
                            
                            sample.IsGroundContact = groundModeByte == 1;
                            sample.IsReactorGroundMode = groundModeByte == 3;


                            // Slip?
                            bufferMs.Position = 32;
                            var slipCoefTest = bufferR.ReadUInt16();
                            Console.WriteLine(slipCoefTest);
                            bufferMs.Position = 33;
                            bufferMs.Position = 34;

                            Console.WriteLine($"5:1 RPM {rpmByte} 14:1 Steer {steerByte}-{steer} 91:1 Gear {gearByte}-{gear} 15:1 u15 {u15} 18:1 brake/gas {brakeByte}-{brake}/{gas} 47:22 prsv {position}-{rotation}-{speed}-{velocity}");

                            break;
                        }
                    case 10:
                        break;
                    default:
                        break;
                }

                samples.Add(sample);
            }

            u04 = r.ReadByte();

            if (version >= 2)
            {
                while (r.ReadByte() != 0)
                {
                    var type = r.ReadInt32();
                    var timestamp = r.ReadInt32();
                    var buffer = r.ReadBytes(); // MwBuffer
                }
            }
        }

        if (version >= 3)
        {
            while (r.ReadByte() != 0)
            {
                var u19 = r.ReadInt32();
                var u20 = r.ReadInt32();
                var u21 = r.ReadBytes(); // MwBuffer
            }

            if (version == 7)
            {
                while (r.ReadByte() != 0)
                {
                    var u23 = r.ReadInt32();
                    var u24 = r.ReadBytes(); // MwBuffer
                }
            }

            if (version >= 8)
            {
                var u23 = r.ReadInt32();

                if (u23 == 0)
                {
                    return samples;
                }

                if (version == 8)
                {
                    while (r.ReadByte() != 0)
                    {
                        var u25 = r.ReadInt32();
                        var u26 = r.ReadBytes(); // MwBuffer
                    }
                }
                else
                {
                    while (r.ReadByte() != 0)
                    {
                        var u28 = r.ReadInt32();
                        var u29 = r.ReadBytes(); // MwBuffer
                        var u30 = r.ReadBytes(); // MwBuffer
                    }

                    if (version >= 10)
                    {
                        var period = r.ReadInt32();
                    }
                }
            }
        }

        return samples;
    }

    /// <summary>
    /// CPlugEntRecordData 0x000 chunk
    /// </summary>
    [Chunk(0x0911F000)]
    public class Chunk0911F000 : Chunk<CPlugEntRecordData>, IVersionable
    {       
        public int Version { get; set; }

        public int CompressedSize { get; private set; }
        public int UncompressedSize { get; private set; }

        public override void Read(CPlugEntRecordData n, GameBoxReader r)
        {
            Version = r.ReadInt32(); // 10
            UncompressedSize = r.ReadInt32();
            CompressedSize = r.ReadInt32();
            n.Data = r.ReadBytes(CompressedSize);
        }

        public override void Write(CPlugEntRecordData n, GameBoxWriter w)
        {
            w.Write(Version);
            w.Write(UncompressedSize);
            w.Write(CompressedSize);
            w.Write(n.Data);
        }
    }
}
