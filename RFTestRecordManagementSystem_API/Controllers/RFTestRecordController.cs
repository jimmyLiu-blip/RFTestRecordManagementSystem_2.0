using RFTestRecordManagementSystem_Domain;
using RFTestRecordManagementSystem_Repository;
using RFTestRecordManagementSystem_Service;
using System.Collections.Generic;
using System.Web.Http; // 使用API時要用到
using static System.Net.WebRequestMethods;

namespace RFTestRecordManagementSystem_API.Controllers
{
    // ApiController 是 ASP.NET Web API 框架內建的基底類別
    // 讓控制器具備「Web API 特性」，例如：
    // 1.自動解析 HTTP 請求（GET, POST, PUT, DELETE）
    // 2.自動回傳 JSON 格式資料
    // 3.支援屬性路由[HttpGet], [HttpPost], [Route] 等
    // 4.支援回傳 HTTP 狀態碼（Ok(), NotFound(), BadRequest()）
    public class RFTestRecordController : ApiController
    {
        private readonly IRFTestRecordService _service;

        public RFTestRecordController()
        {
            var _repository = new DapperRFTestRecordRepository();
            _service = new RFTestRecordService(_repository);      
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
                return NotFound(); // 回傳 404
            }
            return Ok(record);     // 回傳 200 + Json
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
