using LT.DigitalOffice.RightsService.Broker.Consumers;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
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

namespace LT.DigitalOffice.RightsService.Broker.UnitTests.Consumers
{
    class AccessCollectionValidatorConsumerTests
    {
        private InMemoryTestHarness _harness;
        private Mock<ICheckRightsRepository> _rigthsRepositoryMock;
        private ConsumerTestHarness<AccessCollectionValidatorConsumer> _consumerTestHarness;
        private Guid _userGuidWithRights;
        private Guid _userGuidWithoutRights;

        //[SetUp]
        //public void SetUp()
        //{
        //    _harness = new InMemoryTestHarness();
        //    _rigthsRepositoryMock = new Mock<ICheckRightsRepository>();

        //    _userGuidWithRights = Guid.NewGuid();
        //    _userGuidWithoutRights = Guid.NewGuid();

        //    _rigthsRepositoryMock
        //        .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithRights), It.IsAny<int>()))
        //        .Returns(true);

        //    _rigthsRepositoryMock
        //        .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithoutRights), It.IsAny<int>()))
        //        .Returns(false);

        //    _consumerTestHarness = _harness.Consumer(
        //        () => new AccessCollectionValidatorConsumer(_rigthsRepositoryMock.Object));
        //}

        //[Test]
        //public async Task ShouldSendResponseToBrokerWhenUserHasRights()
        //{
        //    await _harness.Start();

        //    try
        //    {
        //        var requestClient = await _harness.ConnectRequestClient<IAccessValidatorCheckRightsCollectionServiceRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<bool>>(
        //            IAccessValidatorCheckRightsCollectionServiceRequest.CreateObj(_userGuidWithRights, new List<int>() { 0, 1 }));

        //        Assert.True(response.Message.IsSuccess);
        //        Assert.IsNull(response.Message.Errors);
        //        Assert.IsTrue(response.Message.Body);
        //        Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorCheckRightsCollectionServiceRequest>().Any(), Is.True);
        //        Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
        //        _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(2));
        //    }
        //    finally
        //    {
        //        await _harness.Stop();
        //    }
        //}

        //[Test]
        //public async Task ShouldSendResponseToBrokerWhenUserDoesNotHaveRights()
        //{
        //    await _harness.Start();

        //    try
        //    {
        //        var requestClient = await _harness.ConnectRequestClient<IAccessValidatorCheckRightsCollectionServiceRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<bool>>(
        //            IAccessValidatorCheckRightsCollectionServiceRequest.CreateObj(_userGuidWithoutRights, new List<int>() { 0, 1 }));

        //        Assert.True(response.Message.IsSuccess);
        //        SerializerAssert.AreEqual(response.Message.Errors, null);
        //        Assert.IsFalse(response.Message.Body);
        //        Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorCheckRightsCollectionServiceRequest>().Any(), Is.True);
        //        Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
        //        _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(1));
        //    }
        //    finally
        //    {
        //        await _harness.Stop();
        //    }
        //}
    }
}
