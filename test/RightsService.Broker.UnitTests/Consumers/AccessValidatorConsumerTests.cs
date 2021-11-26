using LT.DigitalOffice.RightsService.Broker.Consumers;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit.Testing;
using Moq;
using System;

namespace LT.DigitalOffice.RightsService.Broker.UnitTests.Consumers
{
    class AccessValidatorConsumerTests
    {
        private InMemoryTestHarness _harness;
        private Mock<IRightLocalizationRepository> _rigthsRepositoryMock;
        private ConsumerTestHarness<AccessValidatorConsumer> _consumerTestHarness;
        private Guid _userGuidWithRight;
        private Guid _userGuidWithoutRight;

        //[SetUp]
        //public void SetUp()
        //{
        //    _harness = new InMemoryTestHarness();
        //    _rigthsRepositoryMock = new Mock<ICheckRightsRepository>();

        //    _userGuidWithRight = Guid.NewGuid();
        //    _userGuidWithoutRight = Guid.NewGuid();

        //    _rigthsRepositoryMock
        //        .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithRight), It.IsAny<int>()))
        //        .Returns(true);

        //    _rigthsRepositoryMock
        //        .Setup(x => x.IsUserHasRight(It.Is<Guid>(guid => guid == _userGuidWithoutRight), It.IsAny<int>()))
        //        .Returns(false);

        //    _consumerTestHarness = _harness.Consumer(
        //        () => new AccessValidatorConsumer(_rigthsRepositoryMock.Object));
        //}

        //[Test]
        //public async Task ShouldSendResponseToBrokerWhenUserHasRights()
        //{
        //    await _harness.Start();

        //    try
        //    {
        //        var requestClient = await _harness.ConnectRequestClient<IAccessValidatorRightsServiceRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<bool>>(
        //            IAccessValidatorRightsServiceRequest.CreateObj(_userGuidWithRight, 1));

        //        Assert.True(response.Message.IsSuccess);
        //        Assert.IsNull(response.Message.Errors);
        //        Assert.IsTrue(response.Message.Body);
        //        Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorRightsServiceRequest>().Any(), Is.True);
        //        Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
        //        _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
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
        //        var requestClient = await _harness.ConnectRequestClient<IAccessValidatorRightsServiceRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<bool>>(
        //            IAccessValidatorRightsServiceRequest.CreateObj(_userGuidWithoutRight, 3));

        //        Assert.True(response.Message.IsSuccess);
        //        SerializerAssert.AreEqual(response.Message.Errors, null);
        //        Assert.IsFalse(response.Message.Body);
        //        Assert.That(_consumerTestHarness.Consumed.Select<IAccessValidatorRightsServiceRequest>().Any(), Is.True);
        //        Assert.That(_harness.Sent.Select<IOperationResult<bool>>().Any(), Is.True);
        //        _rigthsRepositoryMock.Verify(repository => repository.IsUserHasRight(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        //    }
        //    finally
        //    {
        //        await _harness.Stop();
        //    }
        //}
    }
}
