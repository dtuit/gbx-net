﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GBX.NET.Engines.Game
{
    [Node(0x0311D000)]
    public class CGameCtnZoneGenealogy : Node
    {
        #region Properties

        [NodeMember]
        public string[] ZoneIds { get; set; }

        [NodeMember]
        public string CurrentZoneId { get; set; }

        [NodeMember]
        public Direction Dir { get; set; }

        [NodeMember]
        public int CurrentIndex { get; set; }

        #endregion

        #region Methods

        public override string ToString() => string.Join(" ", ZoneIds);

        #endregion

        #region Chunks

        #region 0x001 chunk

        [Chunk(0x0311D001)]
        public class Chunk0311D001 : Chunk<CGameCtnZoneGenealogy>
        {
            public override void ReadWrite(CGameCtnZoneGenealogy n, GameBoxReaderWriter rw)
            {
                n.CurrentIndex = rw.Int32(n.CurrentIndex);
                n.Dir = (Direction)rw.Int32((int)n.Dir);
            }
        }

        #endregion

        #region 0x002 chunk

        [Chunk(0x0311D002)]
        public class Chunk0311D002 : Chunk<CGameCtnZoneGenealogy>
        {
            public override void ReadWrite(CGameCtnZoneGenealogy n, GameBoxReaderWriter rw)
            {
                n.ZoneIds = rw.Array(n.ZoneIds, i => rw.Reader.ReadLookbackString(), x => rw.Writer.WriteLookbackString(x));
                n.CurrentIndex = rw.Int32(n.CurrentIndex); // 9
                n.Dir = (Direction)rw.Int32((int)n.Dir);
                n.CurrentZoneId = rw.LookbackString(n.CurrentZoneId);
            }
        }

        #endregion

        #endregion

        #region Debug view

        private class DebugView
        {
            private readonly CGameCtnZoneGenealogy node;

            public string[] ZoneIds => node.ZoneIds;
            public string CurrentZoneId => node.CurrentZoneId;
            public Direction Dir => node.Dir;
            public int CurrentIndex => node.CurrentIndex;

            public ChunkSet Chunks => node.Chunks;

            public DebugView(CGameCtnZoneGenealogy node) => this.node = node;
        }

        #endregion
    }
}
