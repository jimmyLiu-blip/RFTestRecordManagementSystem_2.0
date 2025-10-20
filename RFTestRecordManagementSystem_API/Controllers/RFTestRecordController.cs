using System.Collections.Generic;
using System.Web.Http;
using RFTestRecordManagementSystem_Service;
using RFTestRecordManagementSystem_Domain;
using RFTestRecordManagementSystem_Repository;

namespace RFTestRecordManagementSystem_API.Controllers
{
    public class RFTestRecordController : ApiController
    {
        private readonly IRFTestRecordRepository _repository;
        private readonly IRFTestRecordService _service;

        public RFTestRecordController(IRFTestRecordRepository repository)
        {
            _repository = repository;       
        }

        //GET: api / RFTestRecord
        [HttpGet]
        public IEnumerable<RFTestRecord> GetAll()
        {
            return _service.GetAllRecords();
        }

        //GET: api / RFTestRecord / 5
        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            var record = _service.GetRecordById(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        // POST: api / RFTestRecord
        [HttpPost]
        public IHttpActionResult Create([FromBody] RFTestRecord record)
        {
            if (record == null)
                return BadRequest("Record cannot be null.");

            _service.AddRecord(record.Regulation, record.RadioTechnology, record.Band, record.PowerDbm, record.Result, record.TestDate);
            return Ok("Record created successfully.");
        }

        // PUT: api / RFTestRecord / 5
        [HttpPut]
        public IHttpActionResult Update(int id, [FromBody] RFTestRecord record)
        {
            if (record == null)
                return BadRequest("Record cannot be null.");

            record.RecordId = id;
            _service.UpdateRecord(record.RecordId, record.Regulation, record.RadioTechnology, record.Band, record.PowerDbm, record.Result, record.TestDate);
            return Ok("Record updated successfully.");
        }

        // DELETE: api / RFTestRecord / 5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var record = _service.GetRecordById(id);
            if (record == null)
            {
                return NotFound();
            }
            _service.DeleteRecord(id);
            return Ok("Record deleted successfully.");
        }

        // [HttpGet]、[HttpPost]、[HttpPut]、[HttpDelete] 這些屬性可以明確指定哪個方法對應哪個 HTTP 動詞
    }
}
