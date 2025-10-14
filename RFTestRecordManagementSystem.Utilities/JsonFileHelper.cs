using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data;
using RFTestRecordManagementSystem_Domain;

namespace RFTestRecordManagementSystem.Utilities
{
    public static class JsonFileHelper
    {
        public static void ExportToJson(List<RFTestRecord> data, string filePath)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                string directory = Path.GetDirectoryName(filePath);

                if (string.IsNullOrWhiteSpace(directory))
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory;
                    filePath = Path.Combine(directory, filePath);
                }

                Directory.CreateDirectory(directory);

                string jsonString = JsonSerializer.Serialize(data, options);

                File.WriteAllText(filePath, jsonString, new UTF8Encoding(true));

                File.AppendAllText(Path.Combine(logDirection, "Util_JsonFile_action_log.txt"), $"[{DateTime.Now}]匯出成功：{Path.GetFileName(filePath)} ({data.Count}筆資料){Environment.NewLine}");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Util_JsonFile_error_log.txt"), $"[{DateTime.Now}]匯出失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"匯出Json檔案時發生錯誤，請檢查JsonFile_error_log.txt", ex);
            }
        }

        public static List<RFTestRecord> ImportFromJson(string filePath)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var directory = Path.GetDirectoryName(filePath);

                if (string.IsNullOrWhiteSpace(directory))
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory;
                    filePath = Path.Combine(directory, filePath);
                }

                Directory.CreateDirectory(directory);

                if (!File.Exists(filePath))
                {
                    File.AppendAllText(Path.Combine(logDirection, "Util_JsonFile_error_log.txt"), $"[{DateTime.Now}]匯入失敗：找不到檔案 {filePath}{Environment.NewLine}");
                    return new List<RFTestRecord>();
                }

                var jsonString = File.ReadAllText(filePath);

                var data = JsonSerializer.Deserialize<List<RFTestRecord>>(jsonString) ?? new List<RFTestRecord>();

                File.AppendAllText(Path.Combine(logDirection, "Util_JsonFile_action_log.txt"), $"[{DateTime.Now}]匯入成功：{Path.GetFileName(filePath)} ({data.Count}筆資料){Environment.NewLine}");

                return data;
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Util_JsonFile_error_log.txt"), $"[{DateTime.Now}]匯入失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"匯入Json檔案時發生錯誤，請檢查JsonFile_error_log.txt", ex);
            }
        }
    }
}
