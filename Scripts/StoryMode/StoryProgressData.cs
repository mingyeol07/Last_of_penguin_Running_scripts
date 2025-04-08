using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryProgressData
{
    public int LastStage = 0;
    public List<StageStarData> StageStarArray = new List<StageStarData>(21);
}

[System.Serializable]
public class StageStarData
{
    public bool[] IsGetStars = new bool[3];
}