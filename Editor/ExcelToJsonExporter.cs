using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ExcelToJsonExporter : EditorWindow
{
    private string excelFolder = "Assets/Resources/Story/";
    private string jsonFolder = "Assets/Resources/StoryJson/";

    [MenuItem("Tools/Export Excel to JSON")]
    public static void ShowWindow()
    {
        GetWindow<ExcelToJsonExporter>("Excel To JSON");
    }

    private void OnGUI()
    {
        GUILayout.Label("Excel To JSON Exporter", EditorStyles.boldLabel);
        excelFolder = EditorGUILayout.TextField("Excel Folder", excelFolder);
        jsonFolder = EditorGUILayout.TextField("JSON Folder", jsonFolder);

        if (GUILayout.Button("Export All"))
        {
            ExportAllExcels();
        }
    }

    private void ExportAllExcels()
    {
        if (!Directory.Exists(jsonFolder)) Directory.CreateDirectory(jsonFolder);

        string[] files = Directory.GetFiles(excelFolder, "*.xlsx");
        foreach (var file in files)
        {
            ExportExcel(file);
        }

        AssetDatabase.Refresh();
        Debug.Log("All Excel exported to JSON successfully!");
    }

    private void ExportExcel(string filePath)
    {
        var dataList = ExcelReader.ReadExcel(filePath);
        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogWarning("No data found in Excel: " + filePath);
            return;
        }

        // 包装成对象，Unity JsonUtility 可解析
        var wrapper = new ExcelDataWrapper { items = dataList };

        string json = JsonUtility.ToJson(wrapper, true);
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string jsonPath = Path.Combine(jsonFolder, fileName + ".json");

        File.WriteAllText(jsonPath, json);
        Debug.Log("Exported: " + jsonPath);
    }

    [System.Serializable]
    private class ExcelDataWrapper
    {
        public List<ExcelReader.ExcelData> items;
    }
}
