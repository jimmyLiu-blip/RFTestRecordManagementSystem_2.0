using RFTestRecordManagementSystem_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem_Repository
{
    public interface IRFTestRecordRepository
    {
        void InsertRecord(RFTestRecord record);

        void UpdateRecord(RFTestRecord record);

        void DeleteRecord(int recordId);

        RFTestRecord GetRecordById(int recordId);

        List<RFTestRecord> GetAllRecords();
        
        List<RFTestRecord> SearchRecords(string regulation, string radioTechnology);
    }
}
