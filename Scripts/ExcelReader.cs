using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

public class ExcelReader
{
    [System.Serializable]
    public class ExcelData
    {
        public string speaker;
        public string content;
        public string avatarImageFileName;
        public string vocalAudioFileName;
        public string backgroundImageFileName;
        public string backgroundMusicFileName;
        public string character1Action;
        public string CoordinateX1;
        public string character1Image;
        public string character2Action;
        public string CoordinateX2;
        public string character2Image;
        public string backgroundEffect;
        public string effectValue;
    }

    public static List<ExcelData> ReadExcel(string filePath)
    {
        List<ExcelData> excelData = new List<ExcelData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                bool isFirstRow = true;
                while (reader.Read())
                {
                    if (isFirstRow) { isFirstRow = false; continue; } // skip header

                    ExcelData data = new ExcelData();
                    data.speaker = reader.IsDBNull(0) ? "" : reader.GetValue(0).ToString();
                    data.content = reader.IsDBNull(1) ? "" : reader.GetValue(1).ToString();
                    data.avatarImageFileName = reader.IsDBNull(2) ? "" : reader.GetValue(2).ToString();
                    data.vocalAudioFileName = reader.IsDBNull(3) ? "" : reader.GetValue(3).ToString();
                    data.backgroundImageFileName = reader.IsDBNull(4) ? "" : reader.GetValue(4).ToString();
                    data.backgroundMusicFileName = reader.IsDBNull(5) ? "" : reader.GetValue(5).ToString();
                    data.character1Action = reader.IsDBNull(6) ? "" : reader.GetValue(6).ToString();
                    data.CoordinateX1 = reader.IsDBNull(7) ? "" : reader.GetValue(7).ToString();
                    data.character1Image = reader.IsDBNull(8) ? "" : reader.GetValue(8).ToString();
                    data.character2Action = reader.IsDBNull(9) ? "" : reader.GetValue(9).ToString();
                    data.CoordinateX2 = reader.IsDBNull(10) ? "" : reader.GetValue(10).ToString();
                    data.character2Image = reader.IsDBNull(11) ? "" : reader.GetValue(11).ToString();
                    data.backgroundEffect = reader.IsDBNull(12) ? "" : reader.GetValue(12).ToString();
                    data.effectValue = reader.IsDBNull(13) ? "" : reader.GetValue(13).ToString();

                    excelData.Add(data);
                }
            }
        }

        return excelData;
    }
}