using LT.DigitalOffice.CheckRightsService.Broker.Consumers;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.CheckRightsService.Broker.UnitTests.Consumers
{
    public class OperationResult : IOperationResult<bool>
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
        public bool Body { get; set; }
    }


    class AccessCollectionValidatorConsumerTests
    {
        private InMemoryTestHarness _harness;
        private Mock<ICheckRightsRepository> _rigthsRepositoryMock;
        private ConsumerTestHarness<AccessCollectionValidatorConsumer> _consumerTestHarness;
        private List<DbRight> _dbRights;
        private Guid _userGuidWithRights;
        private Guid _userGuidWithoutRights;

        [SetUp]
        public void SetUp()
        {
            _harness = new InMemoryTestHarness();
            _rigthsRepositoryMock = new Mock<ICheckRightsRepository>();

            _userGuidWithRights = Guid.NewGuid();
            _userGuidWithoutRights = Guid.NewGuid();

            _dbRights = new List<DbRight>
            {
                new DbRight
                {
                    Id = 0,
                    Name = "Right0",
                    Description = "Discr0"
                },
                new DbRight
                {
                    Id = 1,
                    Name = "Right1",
                    Description = "Discr1"
                }
            };

            _rigthsRepositoryMock
                .Setup(x => x.GetRightsList())
                .Returns(_dbRights);

            _rigthsRepositoryMock
                .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithRights), It.IsAny<int>()))
                .Returns(true);

            _rigthsRepositoryMock
                .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithoutRights), It.IsAny<int>()))
                .Returns(false);

            _consumerTestHarness = _harness.Consumer(
                () => new AccessCollectionValidatorConsumer(_rigthsRepositoryMock.Object));
        }

        [Test]
        public async Task ShouldSendResponseToBrokerWhenUserHasRights()
        {
            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IAccessValidatorCheckRightsCollectionServiceRequest>();

                var response = await requestClient.GetResponse<IOperationResult<bool>>(
                    IAccessValidatorCheckRightsCollectionServiceRequest.CreateObj(_userGuidWithRights, new List<int>() { 0, 1 }));

                Assert.True(response.Message.IsSuccess);
                Assert.IsNull(response.Message.Errors);
                Assert.IsTrue(response.Message.Body);
                Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorCheckRightsCollectionServiceRequest>().Any(), Is.True);
                Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
                _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(2));
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldSendResponseToBrokerWhenUserDoesNotHaveRights()
        {
            await _harness.Start();

            var expectedErrors = new List<string>()
            {
                "Such user doesn't exist or does not have this rights."
            };

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IAccessValidatorCheckRightsCollectionServiceRequest>();

                var response = await requestClient.GetResponse<IOperationResult<bool>>(
                    IAccessValidatorCheckRightsCollectionServiceRequest.CreateObj(_userGuidWithoutRights, new List<int>() { 0, 1 }));

                Assert.False(response.Message.IsSuccess);
                SerializerAssert.AreEqual(response.Message.Errors, expectedErrors);
                Assert.IsFalse(response.Message.Body);
                Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorCheckRightsCollectionServiceRequest>().Any(), Is.True);
                Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
                _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(1));
            }
            finally
            {
                await _harness.Stop();
            }
        }
    }
}
