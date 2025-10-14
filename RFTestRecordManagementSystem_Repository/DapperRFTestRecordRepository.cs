using RFTestRecordManagementSystem_Domain;
using RFTestRecordManagementSystem.Infrastructure;
using Dapper;
using RFTestRecordManagementSystem.Utilities;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace RFTestRecordManagementSystem_Repository
{
    public class DapperRFTestRecordRepository : IRFTestRecordRepository
    {
        public void InsertRecord(RFTestRecord record)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            const string sql = @"INSERT INTO dbo.RFTestRecords 
                                 (Regulation, RadioTechnology, Band, PowerDbm, Result, TestDate)
                                 VALUES (@Regulation, @RadioTechnology, @Band, @PowerDbm, @Result, @TestDate)
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();
                    // ExecuteScalar<int>是Dapper的擴充方法
                    record.RecordId = cn.ExecuteScalar<int>(sql, record);
                    if (record.RecordId <= 0)
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]新增失敗，未取得有效RecordId{Environment.NewLine}");
                        throw new InvalidOperationException($"新增失敗，未能取得有效的RecordId");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]新增成功，{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]新增失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"新增失敗，出現異常錯誤", ex);
            }


        }

        public void UpdateRecord(RFTestRecord record)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            const string sql = @"UPDATE dbo.RFTestRecords SET
                                 Regulation = @Regulation,
                                 RadioTechnology = @RadioTechnology,
                                 Band = @Band,
                                 PowerDbm = @PowerDbm,
                                 Result = @Result,
                                 TestDate = @TestDate
                                 WHERE RecordId = @RecordId";
            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();
                    // Execute()是Dapper專門給「INSERT / UPDATE / DELETE」用的
                    // 它會回傳一個 int 表示「受影響的列數」
                    int affectedRows = cn.Execute(sql, record);
                    if (affectedRows == 0)
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]更新失敗：找不到RecordId={record.RecordId}{Environment.NewLine}");
                        throw new InvalidOperationException($"更新失敗，找不到RecordId={record.RecordId}");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]更新成功：RecordId={record.RecordId}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]更新失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"更新失敗，出現異常錯誤", ex);
            }
        }

        public void DeleteRecord(int recordId)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();
            const string sql = @"DELETE FROM dbo.RFTestRecords
                                 WHERE RecordId = @RecordId";
            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();

                    int affectedRows = cn.Execute(sql, new { RecordId = recordId });

                    if (affectedRows == 0)
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]刪除失敗：找不到RecordId={recordId}{Environment.NewLine}");

                        throw new InvalidOperationException($"找不到RecordId={recordId}的紀錄");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]刪除成功：RecordId={recordId}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {

                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]刪除失敗：{ex.Message}{Environment.NewLine}");
                throw new InvalidOperationException($"刪除失敗，出現異常錯誤", ex);
            }

        }

        public RFTestRecord GetRecordById(int recordId)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();

            const string sql = @"SELECT RecordId, Regulation, RadioTechnology, Band, PowerDbm, Result, TestDate
                                 FROM dbo.RFTestRecords
                                 WHERE RecordId = @RecordId";
            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();

                    var record = cn.QuerySingleOrDefault<RFTestRecord>(sql, new { RecordId = recordId });

                    if (record == null)
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：找不到RecordId：{recordId}{Environment.NewLine}");

                        throw new InvalidOperationException($"查詢失敗，找不到RecordId={recordId}的紀錄");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]查詢成功：找到RecordId：{recordId}{Environment.NewLine}");

                        return record;
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：{ex.Message}{Environment.NewLine}");

                throw new InvalidOperationException($"查詢失敗，出現異常錯誤", ex);
            }

        }

        public List<RFTestRecord> GetAllRecords()
        {
            var logDirection = LogsHelper.EnsureLogDirectory();

            const string sql = @"SELECT RecordId, Regulation, RadioTechnology, Band, PowerDbm, Result, TestDate
                                 FROM dbo.RFTestRecords
                                 ORDER BY RecordId";

            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();

                    var records = cn.Query<RFTestRecord>(sql).ToList();

                    if (!records.Any())
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：目前沒有任何紀錄{Environment.NewLine}");

                        throw new InvalidOperationException($"查詢失敗，目前沒有任何紀錄");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]查詢成功：共{records.Count}筆{Environment.NewLine}");

                        return records;
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：{ex.Message}{Environment.NewLine}");

                throw new InvalidOperationException($"查詢失敗，出現異常錯誤", ex);
            }
        }

        public List<RFTestRecord> SearchRecords(string regulation, string radioTechnology)
        {
            var logDirection = LogsHelper.EnsureLogDirectory();

            try
            {
                using (var cn = DatabaseConfigurement.GetConnection())
                {
                    cn.Open();

                    var sql = @"SELECT RecordId, Regulation, RadioTechnology, Band, PowerDbm, Result, TestDate
                                FROM dbo.RFTestRecords
                                WHERE 1 = 1";

                    var parameters = new DynamicParameters();

                    if (!string.IsNullOrWhiteSpace(regulation))
                    {
                        // %keyword% 做模糊搜尋
                        sql += " AND LOWER(Regulation) LIKE LOWER(@Regulation)";

                        parameters.Add("@Regulation", $"%{regulation}%");
                    }

                    if (!string.IsNullOrWhiteSpace(radioTechnology))
                    {
                        sql += " AND LOWER(RadioTechnology) LIKE LOWER(@RadioTechnology)";

                        parameters.Add("@RadioTechnology", $"%{radioTechnology}%");
                    }

                    var records = cn.Query<RFTestRecord>(sql, parameters).ToList();

                    if (!records.Any())
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：目前沒有任何紀錄{Environment.NewLine}");

                        throw new InvalidOperationException($"查詢失敗，目前沒有紀錄");
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_action_log.txt"), $"[{DateTime.Now}]查詢成功：共{records.Count}筆{Environment.NewLine}");

                        return records;
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(logDirection, "Repo_Dapper_error_log.txt"), $"[{DateTime.Now}]查詢失敗：{ex.Message}{Environment.NewLine}");

                throw new InvalidOperationException($"查詢失敗，出現異常錯誤", ex);
            }
        }
    }
}
