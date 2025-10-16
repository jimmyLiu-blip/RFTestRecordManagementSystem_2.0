using Moq;
using RFTestRecordManagementSystem_Domain;
using RFTestRecordManagementSystem_Repository;
using RFTestRecordManagementSystem_Service;
using System;
using Xunit;

namespace RFTestRecordManagementSystem.Tests
{
    public class UnitTest1
    {
        [Theory(DisplayName = "AddRecord() 應在 regulation 為空時時丟出例外")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddRecord_ShouldThrowArgumentException_WhenRegulationIsEmpty(string regulation)
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);
            var exception = Assert.Throws<ArgumentException>(() => 
                service.AddRecord(regulation, "NR", "B1", 23, "Pass", DateTime.Now));

            Assert.Contains("Regulation 不可以為空", exception.Message);
        }

        [Theory(DisplayName = "AddRecord() 應在 PowerDbm 超出範圍時丟出例外")]
        [InlineData(60)]
        [InlineData(-60)]
        public void AddRecord_ShouldThrowArgumentOutOfRangeException_WhenPowerDbmIsOutofRange(decimal powerDbm)
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);
            var exception = Assert.Throws<ArgumentOutOfRangeException>( () =>
                service.AddRecord("FCC", "NR", "B2", powerDbm, "Fail", DateTime.Today));

            Assert.Contains("PowerDbm 超出合理範圍", exception.Message);
        }

        [Theory(DisplayName = "AddRecord() 應在 PowerDbm 沒有超出範圍時不丟出例外")]
        [InlineData(45)]
        [InlineData(0)]
        [InlineData(-30)]
        public void AddRecord_ShouldNotThrowException_WhenPowerDbmIsInRange(decimal powerDbm)
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);
            // Record.Exception() 是 xUnit提供的一個輔助方法，用來捕捉某段程式碼在執行時是否拋出了例外
            // var exception = Record.Exception(() => 要執行的動作);
            // Record.Exception 會執行括號內的 Lambda 表達式（() => {...}）
            // 如果那段程式碼 拋出例外，它會 回傳該 Exception 物件
            // 如果 沒有拋出例外，它會回傳 null
            var exception = Record.Exception(() =>
                service.AddRecord("FCC", "NR", "B2", powerDbm, "Fail", DateTime.Today));

            Assert.Null(exception);
        }

        [Theory(DisplayName = "AddRecord() 應在 TestDate 超出範圍時丟出例外")]
        [InlineData("2035-10-15")]
        public void AddRecord_ShouldThrowArgumentOutOfRangeException_WhenTestDateIsOutofRange(string testDateString)
        {
            var testDate = DateTime.Parse(testDateString);
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService (mockRepo.Object);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddRecord("FCC", "NR", "B2", 23, "Fail", testDate));

            Assert.Contains("TestDate 不可為預設值或晚於今日", exception.Message);
        }

        [Fact(DisplayName = "AddRecord() 應在 TestDate 等於預設值時丟出例外")]
        public void AddRecord_ShouldThrowArgumentOutOfRangeException_WhenTestDateDefault()
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddRecord("FCC", "NR", "B2", 23, "Fail", default(DateTime)));

            Assert.Contains("TestDate 不可為預設值或晚於今日", exception.Message);
        }

        [Fact(DisplayName = "AddRecord() 應正確呼叫 Repository.InsertRecord 並正確設定欄位")]

        public void AddRecord_ShouldInsertRecordWithCorrectData_WhenInputIsValid()
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);

            // 設定這個模擬方法可以被驗證(Verifiable())，之後可以用Verify()來確認它有沒有被呼叫過
            mockRepo.Setup( r => r.InsertRecord(It.IsAny<RFTestRecord>())).Verifiable();

            service.AddRecord("FCC", "NR", "B1", 20, "Pass", DateTime.Today);

            mockRepo.Verify( r => r.InsertRecord(
                It.Is<RFTestRecord>(rec => 
                    rec.Regulation == "FCC" &&
                    rec.RadioTechnology == "NR" &&
                    rec.Band == "B1" &&
                    rec.PowerDbm == 20 &&
                    rec.Result == "Pass" &&
                    rec.TestDate == DateTime.Today
                    )), Times.Once);
        }

        [Fact(DisplayName = "AddRecord() 在輸入驗證失敗時不應呼叫 Repository.InsertRecord")]

        public void AddRecord_ShouldNotCallInsertRecord_WhenInputIsInvalid()
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();
            var service = new RFTestRecordService(mockRepo.Object);

            // 驗證外部結果/例外
            Assert.Throws<ArgumentException>(() => service.AddRecord("", "NR", "B1", 20, "Pass", DateTime.Today));
            // 驗證內部方法(是否呼叫方法、幾次、傳入什麼)
            mockRepo.Verify(r => r.InsertRecord(It.IsAny<RFTestRecord>()), Times.Never);
        }
    }
}
