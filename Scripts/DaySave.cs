using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DayData
{
    public List<bool> dayActive;
}

public static class DaySaveSystem
{
    private static string savePath = Application.persistentDataPath + "/daySave.json";

    public static void Save(List<bool> dayActive)
    {
        DayData data = new DayData();
        data.dayActive = dayActive;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("已保存 dayActive 到：" + savePath);
    }

    public static List<bool> Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            DayData data = JsonUtility.FromJson<DayData>(json);
            Debug.Log("已读取 dayActive");
            return data.dayActive;
        }
        else
        {
            Debug.Log("没有存档文件，返回空列表");
            return new List<bool>();
        }
    }

    public static void ResetSave(int dayCount)
    {
        List<bool> newDays = new List<bool>();
        for (int i = 0; i < dayCount; i++)
            newDays.Add(false);

        newDays[0] = true;

        Save(newDays);
        Debug.Log("存档已初始化。");
    }
}
