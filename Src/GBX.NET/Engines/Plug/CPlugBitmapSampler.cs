﻿namespace GBX.NET.Engines.Plug;

/// <summary>
/// Bitmap sampler.
/// </summary>
/// <remarks>ID: 0x0907E000</remarks>
[Node(0x0907E000)]
public class CPlugBitmapSampler : CPlug
{
    internal CPlugBitmapSampler()
    {

    }

    /// <summary>
    /// CPlugBitmapSampler 0x002 chunk
    /// </summary>
    [Chunk(0x0907E002)]
    public class Chunk0907E002 : Chunk<CPlugBitmapSampler>
    {
        public string? U01;
        public CMwNod? U02;
        public int? U03;
        public float? U04;

        public override void ReadWrite(CPlugBitmapSampler n, GameBoxReaderWriter rw)
        {
            rw.Id(ref U01);
            rw.NodeRef(ref U02);
            rw.Int32(ref U03); // DoData
            rw.Single(ref U04);
        }
    }

    /// <summary>
    /// CPlugBitmapSampler 0x005 chunk
    /// </summary>
    [Chunk(0x0907E005)]
    public class Chunk0907E005 : Chunk<CPlugBitmapSampler>
    {
        public string? U01;
        public CMwNod? U02;
        public int? U03;
        public float? U04;

        public override void ReadWrite(CPlugBitmapSampler n, GameBoxReaderWriter rw)
        {
            rw.Id(ref U01);
            rw.NodeRef(ref U02);
            rw.Int32(ref U03);
            rw.Single(ref U04);
        }
    }

    /// <summary>
    /// CPlugBitmapSampler 0x006 chunk
    /// </summary>
    [Chunk(0x0907E006)]
    public class Chunk0907E006 : Chunk<CPlugBitmapSampler>
    {
        public string? U01;
        public CMwNod? U02;
        public int? U03;
        public float? U04;

        public override void ReadWrite(CPlugBitmapSampler n, GameBoxReaderWriter rw)
        {
            rw.Id(ref U01);
            rw.NodeRef(ref U02);
            rw.Int32(ref U03);
            rw.Single(ref U04);
        }
    }

    #region 0x008 chunk

    /// <summary>
    /// CPlugBitmapSampler 0x008 chunk
    /// </summary>
    [Chunk(0x0907E008)]
    public class Chunk0907E008 : Chunk<CPlugBitmapSampler>
    {
        public string? U01;
        public CMwNod? U02;
        public uint U03;
        public float U04;
        public uint? U05;

        public override void ReadWrite(CPlugBitmapSampler n, GameBoxReaderWriter rw)
        {
            rw.Id(ref U01);
            rw.NodeRef(ref U02);
            rw.UInt32(ref U03);
            rw.Single(ref U04);

            // CPlugBitmapSampler::IsBorderColorUsed
            if (((((byte)U03 & 0x18) != 0x18) && (((byte)U03 & 0x60) != 0x60)) && ((U03 & 0x1800) != 0x1800))
            {
                return;
            }

            rw.UInt32(ref U05);
        }
    }

    #endregion
}