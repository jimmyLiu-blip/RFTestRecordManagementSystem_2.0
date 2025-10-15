using Moq;
using RFTestRecordManagementSystem_Repository;
using RFTestRecordManagementSystem_Service;
using System;
using Xunit;

namespace RFTestRecordManagementSystem.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddRecord_ShouldThrowArgumentException_WhenRegulationIsNull(string regulation)
        {
            var mockRepo = new Mock<IRFTestRecordRepository>();

            var service = new RFTestRecordService(mockRepo.Object);

            Assert.Throws<ArgumentException>(() => 
                service.AddRecord(regulation, "NR", "B1", 23, "Pass", DateTime.Now));
        }


    }
}
