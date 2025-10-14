using RFTestRecordManagementSystem.Utilities;
using RFTestRecordManagementSystem_Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem_Repository
{
    public class JsonRFTestRecordRepository : IRFTestRecordRepository
    {
        private readonly string _jsonPath;

        public JsonRFTestRecordRepository()
        {
            // 從 App.config 讀取相對路徑設定（例如 "data\\records.json"）
            string relativePath = ConfigurationManager.AppSettings["JsonFilePath"];
            // 取得目前程式執行檔所在的資料夾路徑（通常是 bin\Debug 或 bin\Release）
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            // 把 basePath + 相對路徑 合併成「完整路徑」
            _jsonPath = Path.Combine(basePath, relativePath);
            // 如果這個 JSON 檔所在的資料夾不存在，就自動建立它
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath));
        }

        public void SaveRecords(List<RFTestRecord> records)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                var jsonString = JsonSerializer.Serialize(records, options);
                File.WriteAllText(_jsonPath, jsonString, new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                var logDirection = LogsHelper.EnsureLogDirectory();
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}]存檔失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"資料存檔時發生錯誤，請檢查JsonDB_error_log.txt", ex);
            }
        }

        public List<RFTestRecord> LoadRecord()
        {
            try
            {
                if (!File.Exists(_jsonPath))
                {
                    return new List<RFTestRecord>();
                }

                var jsonString = File.ReadAllText(_jsonPath);
                return JsonSerializer.Deserialize<List<RFTestRecord>>(jsonString) ?? new List<RFTestRecord>();
            }
            catch (Exception ex)
            {
                var logDirection = LogsHelper.EnsureLogDirectory();
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}]讀檔失敗：{ex.Message}{Environment.NewLine}");
                return new List<RFTestRecord>();
            }
        }

        public void InsertRecord(RFTestRecord record)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var records = LoadRecord();
                /* if (records.Any(r => r.RecordId == record.RecordId))
                 {
                     throw new InvalidOperationException($"RecordId：{record.RecordId}已存在，新增失敗");
                 }*/
                record.RecordId = records.Count > 0 ? records.Max(r => r.RecordId) + 1 : 1;
                records.Add(record);
                SaveRecords(records);
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_action_log.txt"), $"[{DateTime.Now}]已新增，RecordId：{record.RecordId}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}]新增失敗，發生異常錯誤{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"新增資料時發生錯誤，請檢查JsonDB_error_log.txt", ex);
            }

        }

        public void UpdateRecord(RFTestRecord record)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var records = LoadRecord();

                var existingRecord = records.FirstOrDefault(r => r.RecordId == record.RecordId);

                if (existingRecord == null)
                {
                    throw new InvalidOperationException($"找不到RecordId：{record.RecordId}的紀錄，無法更新");
                }

                existingRecord.Regulation = record.Regulation;
                existingRecord.RadioTechnology = record.RadioTechnology;
                existingRecord.Band = record.Band;
                existingRecord.PowerDbm = record.PowerDbm;
                existingRecord.Result = record.Result;
                existingRecord.TestDate = record.TestDate;
                SaveRecords(records);
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_action_log.txt"), $"[{DateTime.Now}]已更新，RecordId：{record.RecordId}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}]更新失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"更新資料時發生錯誤，請檢查JsonDB_error_log.txt", ex);
            }
        }

        public void DeleteRecord(int recordId)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var records = LoadRecord();

                var record = records.FirstOrDefault(r => r.RecordId == recordId);

                if (record == null)
                {
                    throw new InvalidOperationException($"找不到RecordId：{recordId}的紀錄，無法刪除");
                }

                records.Remove(record);

                SaveRecords(records);
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_action_log.txt"), $"[{DateTime.Now}]已刪除，RecordId：{recordId}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}]刪除失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"刪除資料時發生錯誤，請檢查JsonDB_error_log.txt", ex);
            }
        }

        public RFTestRecord GetRecordById(int recordId)
        {
            var record = LoadRecord().FirstOrDefault(r => r.RecordId == recordId);

            if (record == null)
            {
                throw new InvalidOperationException($"找不到RecordId：{recordId}的紀錄");
            }
            return record;
        }

        public List<RFTestRecord> GetAllRecords()
        {
            return LoadRecord()
                .OrderBy(r => r.RecordId)
                .ToList();
        }

        public List<RFTestRecord> SearchRecords(string regulation, string radioTechnology)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            try
            {
                var records = LoadRecord();

                var filteredRecords = records.Where(r =>
                    (string.IsNullOrWhiteSpace(regulation) || r.Regulation.IndexOf(regulation, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (string.IsNullOrWhiteSpace(radioTechnology) || r.RadioTechnology.IndexOf(radioTechnology, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();

                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_action_log.txt"), $"[{DateTime.Now}]查詢紀錄：Regulation = {regulation},RadioTechnology = {radioTechnology}{Environment.NewLine}");

                return filteredRecords;
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Json_error_log.txt"), $"[{DateTime.Now}查詢失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"查詢資料發生錯誤，請檢查JsonDB_error_log.txt", ex);
            }
        }
    }
}
