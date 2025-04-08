using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class StageJsonSave
{
    public static void SaveData(StageData stageData, int stageIndex)
    {
        string directoryPath = Application.dataPath + "/Resources/Data";
        string filePath = directoryPath + "/StageData_" + stageIndex + ".json";

        // 디렉터리가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        Debug.Log("File path: " + filePath);
        string json = JsonUtility.ToJson(stageData, true);
        File.WriteAllText(filePath, json);
    }

    public static StageData LoadData(int stageIndex)
    {
        TextAsset test = Resources.Load<TextAsset>("Data/StageData_" + stageIndex);
        if (test != null)
        {
            string json = test.text;
            return JsonUtility.FromJson<StageData>(json);
        }
        else
        {
            Debug.Log("No save file found.");
            return new StageData(); // 빈 StageData 객체 반환
        }
    }
}

