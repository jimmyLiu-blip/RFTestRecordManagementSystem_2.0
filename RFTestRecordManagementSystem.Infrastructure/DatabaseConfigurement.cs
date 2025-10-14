using System.Data.SqlClient;
using RFTestRecordManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem.Infrastructure
{
    public static class DatabaseConfigurement
    {
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["RFTestRecordManagementSystemDB"]?.ConnectionString
            ?? throw new InvalidOperationException("找不到'RFTestRecordManagementSystemDB'的連線字串設定");

        public static SqlConnection GetConnection()
        {
            try
            {
                // 這裡不使用using，因為使用的話，外部一拿到這個物件就無效了
                var connection = new SqlConnection(_connectionString);

                return connection;
            }
            catch (Exception ex)
            {
                var logDirection = LogsHelper.EnsureLogDirectory();

                File.AppendAllText(Path.Combine(logDirection, "Infra_DB_error_log.txt"), $"[{DateTime.Now}]資料庫連線失敗：{ex.Message}{Environment.NewLine}");

                throw new InvalidOperationException("建立資料庫連線時發生錯誤，請確認 App.config 連線字串是否正確", ex);
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {

                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                var logDirection = LogsHelper.EnsureLogDirectory();

                File.AppendAllText(Path.Combine(logDirection, "Infra_DB_error_log.txt"), $"[{DateTime.Now}]資料庫連線失敗：{ex.Message}{Environment.NewLine}");

                throw new InvalidOperationException($"資料庫連線失敗：{ex.Message}", ex);
            }
        }

    }

}

