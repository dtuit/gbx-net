﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GBX.NET.Engines.Game
{
    [Node(0x030A4000)]
    public class CGameCtnMediaBlockCameraEffectShake : CGameCtnMediaBlockCameraEffect
    {
        public Key[] Keys
        {
            get => GetValue<Chunk000>(x => x.Keys) as Key[];
            set => SetValue<Chunk000>(x => x.Keys = value);
        }

        public CGameCtnMediaBlockCameraEffectShake(ILookbackable lookbackable, uint classID) : base(lookbackable, classID)
        {

        }

        [Chunk(0x030A4000)]
        public class Chunk000 : Chunk
        {
            public Key[] Keys { get; set; }

            public Chunk000(CGameCtnMediaBlockCameraEffectShake node) : base(node)
            {
               
            }

            public override void ReadWrite(GameBoxReaderWriter rw)
            {
                Keys = rw.Array(Keys, i =>
                {
                    var time = rw.Reader.ReadSingle();
                    var intensity = rw.Reader.ReadSingle();
                    var speed = rw.Reader.ReadSingle();

                    return new Key(time, intensity, speed);
                },
                x =>
                {
                    rw.Writer.Write(x.Time);
                    rw.Writer.Write(x.Intensity);
                    rw.Writer.Write(x.Speed);
                });
            }
        }

        public class Key
        {
            public float Time { get; }
            public float Intensity { get; }
            public float Speed { get; }

            public Key(float time, float intensity, float speed)
            {
                Time = time;
                Intensity = intensity;
                Speed = speed;
            }
        }
    }
}
