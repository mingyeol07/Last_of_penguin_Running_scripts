using System;
using System.Collections.Generic;
using UnityEngine;
using Lop.Game.Tile;
using Lop.Editor.JellyClass;

[Serializable]
public class StageData
{
    public int StageId;
    public string StageName;
    public string StageDescription;
    public int BackgroundId;

    public TileData[,] ObjectsInStage;

    public List<IJellyObject> JellyList;
}
