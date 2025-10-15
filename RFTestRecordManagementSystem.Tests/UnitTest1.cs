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

            Assert.Throws<ArgumentException>(() => service.AddRecord("", "NR", "B1", 20, "Pass", DateTime.Today));

            mockRepo.Verify(r => r.InsertRecord(It.IsAny<RFTestRecord>()), Times.Never);
        }
    }
}
