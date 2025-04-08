using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지 별 기록들을 담은 클래스
/// </summary>
[System.Serializable]
public class MyRecordDatasInStage
{
    public List<StageRecordData> RecordInStages = new();
}

/// <summary>
/// 스테이지 안에 기록들을 담는 클래스
/// </summary>
[System.Serializable]
public class StageRecordData
{
    public List<RecordData> RecordDatas = new();
}

/// <summary>
/// 기록
/// </summary>
[System.Serializable]
public class RecordData
{
    public int PenguinID;
    public int Score = 0;
    public string Name = "";
}