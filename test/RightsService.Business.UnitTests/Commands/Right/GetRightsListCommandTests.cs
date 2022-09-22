using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Right;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
  public class GetRightsListCommandTests
  {
    private AutoMocker _mocker;
    private IGetRightsListCommand _command;

    private string _locale;
    private List<DbRightLocalization> _dbRightsLocalizations;
    private DbRightLocalization _dbRight;
    private List<RightInfo> _rightInfos;
    private RightInfo _rightInfo;
    private OperationResultResponse<List<RightInfo>> _badResponse;
    private OperationResultResponse<List<RightInfo>> _goodResponse;

    private void Verifiable(
      Times accessValidatorTimes,
      Times rightInfoMapperTimes,
      Times responseCreatorTimes,
      Times rightLocalizationRepositoryTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x =>
          x.IsAdminAsync(It.IsAny<Guid?>()),
        accessValidatorTimes);

      _mocker.Verify<IResponseCreator, OperationResultResponse<List<RightInfo>>>(x =>
          x.CreateFailureResponse<List<RightInfo>>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorTimes);

      _mocker.Verify<IRightLocalizationRepository, Task<List<DbRightLocalization>>>(x =>
          x.GetRightsListAsync(It.IsAny<string>()),
        rightLocalizationRepositoryTimes);

      _mocker.Verify<IRightInfoMapper, RightInfo>(x =>
          x.Map(It.IsAny<DbRightLocalization>()),
        rightInfoMapperTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<GetRightsListCommand>();

      _locale = "en";

      _dbRight = new()
      {
        Id = Guid.NewGuid(),
        RightId = 0,
        Locale = _locale,
        Name = "Right",
        Description = "Description"
      };

      _dbRightsLocalizations = new List<DbRightLocalization> { _dbRight };

      _rightInfo = new()
      {
        RightId = 0,
        Locale = _locale,
        Name = "Right",
        Description = "Description"
      };

      _rightInfos = new List<RightInfo> { _rightInfo };

      _goodResponse = new()
      {
        Body = _rightInfos
      };

      _badResponse = new()
      {
        Body = null,
        Errors = new List<string> { "Not enough rights." }
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IRightLocalizationRepository>().Reset();
      _mocker.GetMock<IRightInfoMapper>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<List<RightInfo>>>(x =>
          x.CreateFailureResponse<List<RightInfo>>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_badResponse);

      _mocker
        .Setup<IRightLocalizationRepository, Task<List<DbRightLocalization>>>(x => x.GetRightsListAsync(It.IsAny<string>()))
        .ReturnsAsync(_dbRightsLocalizations);

      _mocker
        .Setup<IRightInfoMapper, RightInfo>(x => x.Map(It.IsAny<DbRightLocalization>()))
        .Returns(_rightInfo);
    }

    [Test]
    public async Task SuccessfullyGetRightsList()
    {
      SerializerAssert.AreEqual(_goodResponse, await _command.ExecuteAsync(_locale));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        rightInfoMapperTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        rightLocalizationRepositoryTimes: Times.Once());
    }

    [Test]
    public async Task UserIsNotAdmin()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(_badResponse, await _command.ExecuteAsync(_locale));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        rightInfoMapperTimes: Times.Never(),
        responseCreatorTimes: Times.Once(),
        rightLocalizationRepositoryTimes: Times.Never());
    }
  }
}
