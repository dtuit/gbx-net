﻿namespace GBX.NET.Engines.Game;

//
// Massive thanks to Mystixor and Shweetz for many of the findings!!!
// Without these guys, this would've stayed a mystery.
// Even though the solution was read out of the game code in the end, they gave the important hint.
//

public partial class CGameCtnGhost
{
    /// <summary>
    /// Set of inputs.
    /// </summary>
    public class PlayerInputData : IReadableWritable
    {
        public enum EVersion
        {
            _2017_07_07 = 7,
            _2017_09_12 = 8,
            _2020_04_08 = 11,
            _2020_07_20 = 12
        }

        private EVersion version; // 8 in shootmania, 12 in tm2020
        private int u02;
        private TimeInt32? startOffset;
        private int ticks;
        private byte[] data = Array.Empty<byte>();

        private IList<IInputChange>? inputChanges;

        public EVersion Version { get => version; set => version = value; }
        public int U02 { get => u02; set => u02 = value; }
        public TimeInt32? StartOffset { get => startOffset; set => startOffset = value; }
        public int Ticks { get => ticks; set => ticks = value; }
        public byte[] Data { get => data; set => data = value; }

        public IList<IInputChange> InputChanges
        {
            get
            {
                if (inputChanges is null)
                {
                    var inputEnumerable = version is <= EVersion._2017_09_12
                        ? ProcessShootmaniaInputs()
                        : ProcessTrackmaniaInputs();

#if DEBUG
                    var testList = new List<IInputChange>();

                    try
                    {
                        foreach (var input in inputEnumerable)
                        {
                            testList.Add(input);
                        }
                    }
                    catch
                    {

                    }
#endif

                    inputChanges = inputEnumerable.ToArray();
                }

                return inputChanges;
            }
        }

        public void ReadWrite(GameBoxReaderWriter rw, int version = 0)
        {
            rw.EnumInt32<EVersion>(ref this.version); // 8 in shootmania, 12 in tm2020
            rw.Int32(ref u02);

            if (version >= 4)
            {
                rw.TimeInt32Nullable(ref startOffset);
            }

            rw.Int32(ref ticks);
            rw.Bytes(ref data!);
        }

        internal IEnumerable<IInputChange> ProcessShootmaniaInputs()
        {
            var r = new BitReader(data);

            for (var i = 0; i < ticks; i++)
            {
                var different = false;

                var mouseAccuX = default(ushort?);
                var mouseAccuY = default(ushort?);
                var strafe = default(EStrafe?);
                var walk = default(EWalk?);
                var vertical = default(byte?);
                var states = default(int?);

                var sameMouse = r.ReadBit();

                if (!sameMouse)
                {
                    mouseAccuX = r.ReadUInt16();
                    mouseAccuY = r.ReadUInt16();

                    different = true;
                }

                var sameValuesAfter = r.ReadBit();
                var sameValue = sameValuesAfter;

                if (!sameValuesAfter)
                {
                    sameValue = r.ReadBit();
                }

                if (!sameValue)
                {
                    strafe = (EStrafe)r.Read2Bit();
                    if (strafe == EStrafe.Corrupted) throw new Exception("Corrupted inputs. (strafe == 2)");

                    walk = (EWalk)r.Read2Bit();
                    if (walk == EWalk.Corrupted) throw new Exception("Corrupted inputs. (walk == 2)");

                    if (version == EVersion._2017_09_12)
                    {
                        vertical = r.Read2Bit();
                    }

                    different = true;
                }

                if (!sameValuesAfter)
                {
                    sameValue = r.ReadBit();

                    if (!sameValue)
                    {
                        var onlyTriggerAndAction = r.ReadBit();

                        states = onlyTriggerAndAction
                            ? r.Read2Bit()
                            : r.ReadInt32();

                        different = true;
                    }
                }

                if (different)
                {
                    yield return new ShootmaniaInputChange(i, mouseAccuX, mouseAccuY, strafe, walk, vertical, states);
                }
            }

            var theRest = r.ReadToEnd();

            if (theRest.Any(x => x != 0))
            {
                throw new Exception("Input buffer not cleared out completely");
            }
        }

        internal IEnumerable<IInputChange> ProcessTrackmaniaInputs()
        {
            var r = new BitReader(data);

            var started = false;

            for (var i = 0; i < ticks; i++)
            {
                var different = false;

                var states = default(ulong?);
                var mouseAccuX = default(ushort?);
                var mouseAccuY = default(ushort?);
                var steer = default(sbyte?);
                var gas = default(bool?);
                var brake = default(bool?);
                var horn = default(bool?);

                try
                {
                    var sameState = r.ReadBit();

                    if (!sameState)
                    {
                        var onlySomething = r.ReadBit();

                        states = onlySomething
                            ? r.Read2Bit()
                            : r.ReadNumber(bits: version is EVersion._2020_04_08 ? 33 : 34);

                        if (started)
                        {
                            if (states == 0) // 0 only appears for the horn key RELEASE
                            {
                                horn = false;
                            }

                            // the issue here is that horn can never be true throughout the run
                        }
                        else
                        {
                            started = (states & 2) != 0;
                            horn = (states & 64) != 0; // a weird bit that can appear sometimes during the run too
                        }

                        different = true;
                    }

                    var sameMouse = r.ReadBit();

                    if (!sameMouse)
                    {
                        mouseAccuX = r.ReadUInt16();
                        mouseAccuY = r.ReadUInt16();

                        different = true;
                    }

                    // This check is a bit weird, may not work for StormMan gameplay
                    // If starting with horn on, it is included on first tick
                    // If mouse is not plugged, it is also included
                    // In code, this check is presented as '(X - 2 & 0xfffffffd) == 0'

                    if (started)
                    {
                        var sameValue = r.ReadBit();

                        if (!sameValue)
                        {
                            steer = r.ReadSByte();
                            gas = r.ReadBit();
                            brake = r.ReadBit();

                            different = true;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    // TM2020 moment
                }

                if (different)
                {
                    yield return new TrackmaniaInputChange(i, states, mouseAccuX, mouseAccuY, steer, gas, brake, horn);
                }
            }

            var theRest = r.ReadToEnd();

            if (theRest.Any(x => x != 0))
            {
                throw new Exception("Input buffer not cleared out completely");
            }
        }

        public enum EStrafe : byte
        {
            None,
            Left,
            Corrupted,
            Right
        }

        public enum EWalk : byte
        {
            None,
            Forward,
            Corrupted,
            Backward
        }

        public interface IInputChange
        {
            int Tick { get; }
            ushort? MouseAccuX { get; }
            ushort? MouseAccuY { get; }
            ulong? States { get; }
            bool? Respawn { get; }
            bool? Horn { get; }

            TimeInt32 Timestamp { get; }
        }

        public readonly record struct ShootmaniaInputChange(int Tick,
                                                            ushort? MouseAccuX,
                                                            ushort? MouseAccuY,
                                                            EStrafe? Strafe,
                                                            EWalk? Walk,
                                                            byte? Vertical,
                                                            int? States) : IInputChange
        {
            public TimeInt32 Timestamp => new(Tick * 10);

            public bool? IsGunTrigger => States is null ? null : (States & 2) != 0;
            public bool? FreeLook => States is null ? null : (States & 4) != 0;
            public bool? Fly => States is null ? null : (States & 8) != 0;
            public bool? Camera2 => States is null ? null : (States & 32) != 0;
            public bool? Jump => States is null ? null : (States & 128) != 0;
            public bool? IsAction => States is null ? null : (States & 257) != 0;
            public bool? ActionSlot1 => States is null ? null : (States & 1024) != 0;
            public bool? ActionSlot2 => States is null ? null : (States & 2048) != 0;
            public bool? ActionSlot3 => States is null ? null : (States & 4096) != 0;
            public bool? ActionSlot4 => States is null ? null : (States & 8192) != 0;
            public bool? Use1 => States is null ? null : (States & 16896) != 0;
            public bool? Use2 => States is null ? null : (States & 32832) != 0;
            public bool? Menu => States is null ? null : (States & 65536) != 0;
            public bool? Horn => States is null ? null : (States & 1048576) != 0;
            public bool? Respawn => States is null ? null : (States & 2097152) != 0;

            ulong? IInputChange.States => States is null ? null : (ulong)States;
        }

        public readonly record struct TrackmaniaInputChange(int Tick,
                                                            ulong? States,
                                                            ushort? MouseAccuX,
                                                            ushort? MouseAccuY,
                                                            sbyte? Steer,
                                                            bool? Gas,
                                                            bool? Brake,
                                                            bool? Horn = null) : IInputChange
        {
            public TimeInt32 Timestamp { get; } = new(Tick * 10);

            public bool? FreeLook => States is null ? null : (States & 8192) != 0; // bit 13
            public bool? ActionSlot1 => States is null ? null : (States & (1 << 14)) != 0;
            public bool? ActionSlot2 => States is null ? null : (States & 32896) != 0; // bit 15 (and bit 7?)
            public bool? ActionSlot3 => States is null ? null : (States & (1 << 16)) != 0;
            public bool? ActionSlot4 => States is null ? null : (States & 132096) != 0; // bit 17 (and bit 7?)
            public bool? ActionSlot5 => States is null ? null : (States & (1 << 18)) != 0;
            public bool? ActionSlot6 => States is null ? null : (States & 524288) != 0; // bit 19
            public bool? ActionSlot7 => States is null ? null : (States & (1 << 20)) != 0;
            public bool? ActionSlot8 => States is null ? null : (States & 2097152) != 0; // bit 21
            public bool? ActionSlot9 => States is null ? null : (States & (1 << 22)) != 0;
            public bool? ActionSlot0 => States is null ? null : (States & 8388608) != 0; // bit 23
            public bool? Respawn => States is null ? null : (States & 2147483648) != 0;
            public bool? SecondaryRespawn => States is null ? null : (States & 8589934592) != 0;
        }
    }
}
