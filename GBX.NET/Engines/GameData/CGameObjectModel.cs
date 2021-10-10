﻿using GBX.NET.Engines.MwFoundations;

namespace GBX.NET.Engines.GameData
{
    [Node(0x2E01D000)]
    public sealed class CGameObjectModel : CMwNod
    {
        public uint m_InventoryParams_InventoryOccupation { get; set; }

        private CGameObjectModel()
        {

        }

        [Chunk(0x2E01D000)]
        public class Chunk2E01D000 : Chunk<CGameObjectModel>
        {
            public override void ReadWrite(CGameObjectModel n, GameBoxReaderWriter rw)
            {
                rw.Int32();
                rw.Int32();
                rw.Int32();
                rw.Int32();
                rw.Int32();
                rw.Int32();
                rw.Int32();
                n.m_InventoryParams_InventoryOccupation = rw.UInt32(n.m_InventoryParams_InventoryOccupation);
                rw.Int32();
                rw.Int32();
                rw.Int32();
            }
        }
    }
}
