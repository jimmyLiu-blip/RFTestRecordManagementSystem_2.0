using RFTestRecordManagementSystem.Infrastructure;
using RFTestRecordManagementSystem.Utilities;
using RFTestRecordManagementSystem_Repository;
using RFTestRecordManagementSystem_Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem
{

    public class RFTestInput
    {
        public string Regulation { get; set; }
        public string RadioTechnology { get; set; }
        public string Band { get; set; }
        public decimal PowerDbm { get; set; }
        public string Result { get; set; }
        public DateTime TestTime { get; set; }
    }

    public class Program
    {
        private static readonly bool UseJson = false;

        private static readonly IRFTestRecordRepository _repository = UseJson ?
            (IRFTestRecordRepository)new JsonRFTestRecordRepository(): new DapperRFTestRecordRepository();

        private static readonly IRFTestRecordService _service = new RFTestRecordService(_repository);

        static void Main(string[] args)
        {
            Console.WriteLine("正在測試資料庫連線");

            try
            {
                if (!UseJson)
                {
                    DatabaseConfigurement.TestConnection();

                    Console.WriteLine("資料庫連線成功");
                }
                else
                {
                    Console.WriteLine("目前使用 Json儲存(略過資料庫連線測試)");
                }

                Console.WriteLine("按任意鍵進入主選單...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);

                Console.WriteLine("應用程式因資料庫錯誤而中止");

                Console.ResetColor();

                Console.WriteLine("按任意鍵離開...");

                Console.ReadKey();

                return;
            }

            bool exit = false;

            while (!exit)
            {
                try
                {
                    Console.Clear();

                    Console.WriteLine("   ===歡迎使用RF測試記錄管理系統===   ");

                    ShowMenu();

                    string choice = Console.ReadLine();

                    Console.Clear();

                    switch (choice)
                    {
                        case "1":
                            AddRecord();
                            break;
                        case "2":
                            UpdateRecord();
                            break;
                        case "3":
                            DeleteRecord();
                            break;
                        case "4":
                            GetRecordById();
                            break;
                        case "5":
                            GetAllRecords();
                            break;
                        case "6":
                            SearchRecords();
                            break;
                        case "7":
                            ExportToJson();
                            break;
                        case "8":
                            ImportFromJson();
                            break;
                        case "9":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("輸入錯誤，請重新輸入");
                            break;
                    }

                    if (!exit)
                    {
                        Console.WriteLine("按任意鍵繼續...");
                        Console.ReadKey();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"出現操作錯誤{ex.Message}");
                    Console.WriteLine("按任意鍵回到目錄頁");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出現異常錯誤{ex.Message}");
                    Console.WriteLine("按任意鍵回到目錄頁");
                    Console.ReadLine();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("1.新增測試紀錄");

            Console.WriteLine("2.更新測試紀錄");

            Console.WriteLine("3.刪除測試紀錄");

            Console.WriteLine("4.使用測試編號，查詢一筆測試紀錄");

            Console.WriteLine("5.查詢所有測試紀錄");

            Console.WriteLine("6.使用法規、無線技術，查詢測試紀錄");

            Console.WriteLine("7.將資料匯出成Json檔案");

            Console.WriteLine("8.將Json檔案匯入進來");

            Console.WriteLine("9.離開系統");

            Console.Write("請輸入選項(1-9):");
        }

        private static RFTestInput GetRequireInput()
        {
            var input = new RFTestInput();

            Console.Write("請輸入法規(FCC、NCC、IC、TELEC、CE)：");
            input.Regulation = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input.Regulation))
            {
                Console.Write("法規(必填)，請重新輸入：");

                input.Regulation = Console.ReadLine();
            }

            Console.Write("請輸入無線技術(GSM、WCDMA、LTE、NR)：");
            input.RadioTechnology = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input.RadioTechnology))
            {
                Console.Write("無線技術(必填)，請重新輸入：");

                input.RadioTechnology = Console.ReadLine();
            }

            Console.Write("請輸入測試頻段(Band1、Band2...Band106)：");
            input.Band = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input.Band))
            {
                Console.WriteLine("測試頻段(必填)，請重新輸入：");
                input.Band = Console.ReadLine();
            }

            Console.Write("請輸入量測功率(dbm)：");
            decimal powerDbm;
            while (!decimal.TryParse(Console.ReadLine(), out powerDbm) || powerDbm < -50 || powerDbm > 50)
            {
                Console.Write("量測功率超出測試範圍(-50dbm~50dbm)，請重新輸入：");
            }
            input.PowerDbm = powerDbm;

            Console.Write("請輸入測試結果(Pass/Fail)：");
            input.Result = Console.ReadLine();
            while (input.Result.ToUpper() != "PASS" && input.Result.ToUpper() != "FAIL")
            {
                Console.WriteLine("測試結果輸入錯誤，請重新輸入(Pass / Fail)：");
                input.Result = Console.ReadLine();
            }

            Console.Write("請輸入測試日期(yyyy-MM-dd)：");
            DateTime testTime;
            while (!DateTime.TryParse(Console.ReadLine(), out testTime) || testTime > DateTime.Now)
            {
                Console.WriteLine("日期格式錯誤或時間超出範圍，請重新輸入(yyyy-MM-dd)");
            }
            input.TestTime = testTime;

            return input;
        }

        private static void AddRecord()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===新增測試紀錄===   ");
            Console.ResetColor();

            RFTestInput input = GetRequireInput();

            try
            {
                int recordId = _service.AddRecord(input.Regulation, input.RadioTechnology, input.Band, input.PowerDbm, input.Result, input.TestTime);

                Console.WriteLine($"新增測試紀錄成功，RecordId：{recordId}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"新增失敗，{ex.Message}");

                Console.ResetColor();
            }
        }

        private static void UpdateRecord()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===更新測試紀錄===   ");
            Console.ResetColor();

            Console.Write("請輸入需要更新的測試編號：");

            int recordId;

            while (!int.TryParse(Console.ReadLine(), out recordId) || recordId <= 0)
            {
                Console.Write("測試編號格式不對或 <= 0，請重新輸入");
            }

            try
            {
                var record = _service.GetRecordById(recordId);

                if (record == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"找不到RecordId為{recordId}的紀錄");
                    Console.ResetColor();
                    return;
                }

                RFTestInput input = GetRequireInput();

                Console.WriteLine($"\n要更新的紀錄編號：{recordId}，測試資料：");
                Console.WriteLine($"法規：{record.Regulation}");
                Console.WriteLine($"無線技術：{record.RadioTechnology}");
                Console.WriteLine($"頻段：{record.Band}");
                Console.WriteLine($"功率：{record.PowerDbm} dBm");
                Console.WriteLine($"測試結果：{record.Result}");
                Console.WriteLine($"測試日期：{record.TestDate:yyyy-MM-dd}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("確認是否要更新?(Y/N)");
                Console.ResetColor();

                string confirm = Console.ReadLine().Trim().ToUpper();

                if (confirm != "Y")
                {
                    Console.WriteLine("取消更新操作");
                    return;
                }

                _service.UpdateRecord(recordId, input.Regulation, input.RadioTechnology, input.Band, input.PowerDbm, input.Result, input.TestTime);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"更新成功，RecordId為{recordId}");
                Console.ResetColor();
            }
            catch (KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"找不到該筆RecordID：{recordId}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"更新失敗，{ex.Message}");
                Console.ResetColor();
            }
        }

        private static void DeleteRecord()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===刪除測試紀錄===   ");
            Console.ResetColor();

            Console.Write("請輸入想要刪除的測試編號：");

            int recordId;

            while (!int.TryParse(Console.ReadLine(), out recordId) || recordId <= 0)
            {
                Console.Write("輸入格式錯誤或 <= 0，請重新輸入：");
            }

            try
            {
                var record = _service.GetRecordById(recordId);

                if (record == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"找不到RecordId為{recordId}");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"\n以下是{recordId}測試編號的測試紀錄：");
                Console.WriteLine($"法規：{record.Regulation}");
                Console.WriteLine($"無線技術：{record.RadioTechnology}");
                Console.WriteLine($"頻段：{record.Band}");
                Console.WriteLine($"功率：{record.PowerDbm} dBm");
                Console.WriteLine($"測試結果：{record.Result}");
                Console.WriteLine($"測試日期：{record.TestDate:yyyy-MM-dd}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("是否確定要刪除此筆資料?(Y/N)：");
                Console.ResetColor();

                string confirm = Console.ReadLine().Trim().ToUpper();

                if (confirm != "Y")
                {
                    Console.WriteLine("取消刪除的操作");
                    return;
                }

                _service.DeleteRecord(recordId);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n成功刪除測試紀錄，RecordId為{recordId}");
                Console.ResetColor();
            }
            catch (KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"RecordId：{recordId}不存在，無法刪除");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"刪除失敗，{ex.Message}");
                Console.ResetColor();
            }
        }

        private static void GetRecordById()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===使用測試編號，查詢一筆測試紀錄===   ");
            Console.ResetColor();

            Console.Write("請輸入想要查詢測試紀錄的測試編號：");

            int recordId;

            while (!int.TryParse(Console.ReadLine(), out recordId) || recordId <= 0)
            {
                Console.Write("測試編號輸入的格式錯誤or編號 <= 0，請重新輸入：");
            }

            var record = _service.GetRecordById(recordId);

            if (record == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"RecordId：{recordId}的測試紀錄不存在");
                Console.ResetColor();
                return;
            }
            else
            {
                int count = 1;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{count}] 找到紀錄：");
                Console.WriteLine($"  RecordId          = {record.RecordId}");
                Console.WriteLine($"  Regulation        = {record.Regulation}");
                Console.WriteLine($"  RadioTechnology   = {record.RadioTechnology}");
                Console.WriteLine($"  Band              = {record.Band}");
                Console.WriteLine($"  PowerDbm          = {record.PowerDbm}");
                Console.WriteLine($"  Result            = {record.Result}");
                Console.WriteLine($"  TestDate          = {record.TestDate:yyyyMMdd}");
                count++;
                Console.ResetColor();
            }
        }

        private static void GetAllRecords()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===查詢所有測試紀錄===   ");
            Console.ResetColor();

            var records = _service.GetAllRecords();

            if (records == null || !records.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("目前沒有測試紀錄存在");
                Console.ResetColor();
                return;
            }
            else
            {
                int count = 1;
                foreach (var record in records)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{count}] 找到紀錄：");
                    Console.WriteLine($"  RecordId          = {record.RecordId}");
                    Console.WriteLine($"  Regulation        = {record.Regulation}");
                    Console.WriteLine($"  RadioTechnology   = {record.RadioTechnology}");
                    Console.WriteLine($"  Band              = {record.Band}");
                    Console.WriteLine($"  PowerDbm          = {record.PowerDbm}");
                    Console.WriteLine($"  Result            = {record.Result}");
                    Console.WriteLine($"  TestDate          = {record.TestDate:yyyyMMdd}");
                    Console.WriteLine(new string('-', 50));
                    count++;
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"總共有{records.Count}筆測試紀錄");
            Console.ResetColor();
        }

        private static void SearchRecords()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===使用法規、無線技術，查詢測試紀錄===   ");
            Console.ResetColor();

            Console.Write("請輸入欲搜尋的法規(至少填一種regulation、radioTechnology)：");
            string regulation = Console.ReadLine();

            Console.Write("請輸入欲搜尋的無線技術(至少填一種regulation、radioTechnology)：");
            string radioTechnology = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(regulation) && string.IsNullOrWhiteSpace(radioTechnology))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("不可以同時都沒輸入，請重新輸入：");
                Console.ResetColor();

                Console.Write("請輸入法規：");
                regulation = Console.ReadLine();

                Console.Write("請輸入無線技術：");
                radioTechnology = Console.ReadLine();
            }

            var records = _service.SearchRecords(regulation, radioTechnology);

            if (records == null || !records.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"目前沒有Regulation：{regulation}、RadioTechnology：{radioTechnology}測試紀錄存在");
                Console.ResetColor();
            }
            else
            {
                int count = 1;
                foreach (var record in records)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{count}] 找到紀錄：");
                    Console.WriteLine($"  RecordId          = {record.RecordId}");
                    Console.WriteLine($"  Regulation        = {record.Regulation}");
                    Console.WriteLine($"  RadioTechnology   = {record.RadioTechnology}");
                    Console.WriteLine($"  Band              = {record.Band}");
                    Console.WriteLine($"  PowerDbm          = {record.PowerDbm}");
                    Console.WriteLine($"  Result            = {record.Result}");
                    Console.WriteLine($"  TestDate          = {record.TestDate:yyyyMMdd}");
                    Console.WriteLine(new string('-', 50));
                    count++;
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"總共有{records.Count}筆測試紀錄");
            Console.ResetColor();
        }

        private static void ExportToJson()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   === 將資料匯出成 JSON 檔案 ===   ");
            Console.ResetColor();

            try
            {
                var records = _service.GetAllRecords();

                if (records == null || !records.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("目前沒有測試紀錄存在");
                    Console.ResetColor();
                    return;
                }

                string fileName = $"RFTestRecords_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                // 將檔案存到專案目錄下的「Export」資料夾
                string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export", fileName);

                // 確保 Export 資料夾存在（否則會報錯）
                Directory.CreateDirectory(Path.GetDirectoryName(defaultPath));

                Console.Write($"預設儲存路徑為：\n{defaultPath}，是否要修改儲存位置?(Y/N)：");
                string answer = Console.ReadLine().Trim().ToUpper();

                string filePath = defaultPath;
                if (answer == "Y")
                {
                    Console.Write("請輸入要儲存的完整路徑(含檔名.json)：");
                    string inputPath = Console.ReadLine().Trim();
                    if (!string.IsNullOrWhiteSpace(inputPath))
                    {
                        filePath = inputPath;
                    }
                }

                JsonFileHelper.ExportToJson(records, filePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"匯出成功，檔案已經儲存在{filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"匯出失敗，{ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ImportFromJson()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   ===匯入 Json 檔案===   ");
            Console.ResetColor();

            try
            {
                Console.Write("請輸入完整檔案路徑(含檔名.json)：");

                string filePath = Console.ReadLine().Trim();

                while (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"檔案路徑有誤or檔案不存在，請重新輸入：");
                    Console.ResetColor();

                    filePath = Console.ReadLine().Trim().ToUpper();
                }

                var records = JsonFileHelper.ImportFromJson(filePath);

                if (records == null || !records.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("指定檔案中沒有任何可匯入的資料");
                    Console.ResetColor();
                    return;
                }
                else
                {
                    int count = 1;
                    foreach (var record in records)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{count}] 找到紀錄：");
                        Console.WriteLine($"  RecordId          = {record.RecordId}");
                        Console.WriteLine($"  Regulation        = {record.Regulation}");
                        Console.WriteLine($"  RadioTechnology   = {record.RadioTechnology}");
                        Console.WriteLine($"  Band              = {record.Band}");
                        Console.WriteLine($"  PowerDbm          = {record.PowerDbm}");
                        Console.WriteLine($"  Result            = {record.Result}");
                        Console.WriteLine($"  TestDate          = {record.TestDate:yyyyMMdd}");
                        Console.WriteLine(new string('-', 50));
                        count++;
                        Console.ResetColor();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"總共有{records.Count}筆數據");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("是否要將這些資料寫入資料庫中?(Y/N)");
                Console.ResetColor();

                string confirm = Console.ReadLine().Trim().ToUpper();
                if (confirm == "Y")
                {
                    int successCount = 0;
                    int skipCount = 0;
                    foreach (var record in records)
                    {
                        record.RecordId = 0;

                        if (_service.GetRecordById(record.RecordId) != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"RecordId：{record.RecordId}已存在，略過");
                            Console.ResetColor();
                            skipCount++;
                            continue;
                        }
                        else
                        {
                            try
                            {
                                _service.AddRecord(
                                                    record.Regulation,
                                                    record.RadioTechnology,
                                                    record.Band,
                                                    record.PowerDbm,
                                                    record.Result,
                                                    record.TestDate);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"發生異常錯誤，無法將資料新增到資料庫：{ex.Message}");
                                Console.ResetColor();
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"匯入成功，總共成功匯入：{successCount}筆，略過：{skipCount}筆資料，共{records.Count}筆");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"發生異常錯誤，無法順利匯入檔案，{ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
