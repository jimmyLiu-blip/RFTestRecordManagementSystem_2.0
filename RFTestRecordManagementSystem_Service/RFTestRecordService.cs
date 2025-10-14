using RFTestRecordManagementSystem_Domain;
using RFTestRecordManagementSystem_Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem_Service
{
    public class RFTestRecordService : IRFTestRecordService
    {
        private readonly IRFTestRecordRepository _repository;

        public RFTestRecordService(IRFTestRecordRepository repository)
        {
            _repository = repository;
        }

        private void ApplyRecordChanges(RFTestRecord record, string regulation, string radioTechnology, string band, decimal powerDbm, string result, DateTime testDate)
        {
            record.Regulation = regulation.Trim();
            record.RadioTechnology = radioTechnology.Trim();
            record.Band = band.Trim();
            record.PowerDbm = powerDbm;
            record.Result = result.Trim();
            record.TestDate = testDate;
        }

        private void ValidateUserInput(string regulation, string radioTechnology, string band, decimal powerDbm, string result, DateTime testDate)
        {
            if (string.IsNullOrWhiteSpace(regulation))
            {
                throw new ArgumentException("Regulation 不可以為空", nameof(regulation));
            }

            if (string.IsNullOrWhiteSpace(radioTechnology))
            {
                throw new ArgumentException("RadioTechnology 不可以為空", nameof(radioTechnology));
            }

            if (string.IsNullOrWhiteSpace(band))
            {
                throw new ArgumentException("Band不可以為空", nameof(band));
            }

            if (powerDbm < -50 || powerDbm > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(powerDbm), "PowerDbm 超出合理範圍");
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException("Result 不可以為空", nameof(result));
            }

            // 加上 .Date 的效果，只比較「日期部分」，忽略「時間」
            if (testDate == default(DateTime) || testDate > DateTime.Now.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(testDate), "TestDate 不可為預設值或晚於今日");
            }
        }

        public int AddRecord(string regulation, string radioTechnology, string band, decimal powerDbm, string result, DateTime testDate)
        {
            ValidateUserInput(regulation, radioTechnology, band, powerDbm, result, testDate);

            var newRecord = new RFTestRecord();

            ApplyRecordChanges(newRecord, regulation, radioTechnology, band, powerDbm, result, testDate);

            _repository.InsertRecord(newRecord);

            return newRecord.RecordId;
        }

        public void UpdateRecord(int recordId, string regulation, string radioTechnology, string band, decimal powerDbm, string result, DateTime testDate)
        {
            ValidateUserInput(regulation, radioTechnology, band, powerDbm, result, testDate);

            var record = GetRecordById(recordId);

            if (record == null)
            {
                throw new InvalidOperationException($"找不到RecordId為{recordId}的紀錄");
            }

            ApplyRecordChanges(record, regulation, radioTechnology, band, powerDbm, result, testDate);

            _repository.UpdateRecord(record);
        }

        public void DeleteRecord(int recordId)
        {
            if (recordId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(recordId), "紀錄編號必須 > 0");
            }

            var record = GetRecordById(recordId);

            if (record == null)
            {
                throw new InvalidOperationException($"找不到RecordId = {recordId}的紀錄");
            }

            _repository.DeleteRecord(recordId);

        }

        public RFTestRecord GetRecordById(int recordId)
        {
            return _repository.GetRecordById(recordId);
        }

        public List<RFTestRecord> GetAllRecords()
        {
            var records = _repository.GetAllRecords();

            if (records == null || !records.Any())
            {
                throw new InvalidOperationException("目前沒有記錄存在");
            }

            return records;
        }

        public List<RFTestRecord> SearchRecords(string regulation, string radioTechnology)
        {
            if (string.IsNullOrWhiteSpace(regulation) && string.IsNullOrWhiteSpace(radioTechnology))
            {
                throw new InvalidOperationException("至少輸入 Regulation 或 RadioTechnology ");
            }

            var records = _repository.SearchRecords(regulation, radioTechnology);

            if (records == null || !records.Any())
            {
                throw new InvalidOperationException("目前沒有任何相關記錄存在");
            }

            return records;
        }
    }
}
