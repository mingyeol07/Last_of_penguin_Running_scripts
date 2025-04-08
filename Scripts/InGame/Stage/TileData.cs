using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game.Tile
{
    [Serializable]
    public struct TileData
    {
        public int ObjectTypeId;

        public TileData(ObjectType objectType)
        {
            ObjectTypeId = (int)objectType;
        }
    }
}