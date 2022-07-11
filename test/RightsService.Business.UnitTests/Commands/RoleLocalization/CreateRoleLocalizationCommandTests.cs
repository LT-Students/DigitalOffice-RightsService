using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.RoleLocalization
{
  public class CreateRoleLocalizationCommandTests
  {
    private const string RequestIsNotCorrectErrorMessage = "Request is not correct.";
    private const string RoleIdCantBeEmptyErrorMessage = "RoleId can't be empty.";
    private const string NotEnoughRightsErrorMessage = "Not enough rights.";
    private readonly List<string> _roleCanNotBeEmptyErrorList = new() { RoleIdCantBeEmptyErrorMessage };

    private AutoMocker _autoMocker;
    private ICreateRoleLocalizationCommand _command;

    private CreateRoleLocalizationRequest _request;
    private DbRoleLocalization _dbRoleLocalization;

    private void Verifiable(
      Times accessValidatorTimes,
      Times requestValidatorTimes,
      Times mapperTimes,
      Times repositoryTimes)
    {
      _autoMocker.Verify<IAccessValidator>(
        x => x.IsAdminAsync(It.IsAny<Guid?>()),
        accessValidatorTimes);

      _autoMocker.Verify<ICreateRoleLocalizationRequestValidator>(
        x => x.ValidateAsync(It.IsAny<CreateRoleLocalizationRequest>(), It.IsAny<CancellationToken>()),
        requestValidatorTimes);

      _autoMocker.Verify<IDbRoleLocalizationMapper>(
        x => x.Map(It.IsAny<CreateRoleLocalizationRequest>()),
        mapperTimes);

      _autoMocker.Verify<IRoleLocalizationRepository>(
        x => x.CreateAsync(It.IsAny<DbRoleLocalization>()),
        repositoryTimes);

      _autoMocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _command = _autoMocker.CreateInstance<CreateRoleLocalizationCommand>();

      _request = new CreateRoleLocalizationRequest
      {
        RoleId = Guid.NewGuid(),
        Locale = "EN",
        Name = "Name",
        Description = "Description"
      };

      _dbRoleLocalization = new DbRoleLocalization
      {
        Id = Guid.NewGuid(),
        RoleId = _request.RoleId.Value,
        Locale = _request.Locale,
        Name = _request.Name,
        Description = _request.Description,
        CreatedBy = Guid.NewGuid(),
        CreatedAtUtc = DateTime.Now,
        IsActive = true
      };

      _autoMocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(200);

      _autoMocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(
          x => x.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<Guid?>
        {
          Errors = new() { RequestIsNotCorrectErrorMessage }
        });

      _autoMocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(
          x => x.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest, _roleCanNotBeEmptyErrorList))
        .Returns(new OperationResultResponse<Guid?>
        {
          Errors = new() { RoleIdCantBeEmptyErrorMessage }
        });

      _autoMocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(
          x => x.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<Guid?>
        {
          Errors = new() { NotEnoughRightsErrorMessage }
        });
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IAccessValidator>().Reset();
      _autoMocker.GetMock<ICreateRoleLocalizationRequestValidator>().Reset();
      _autoMocker.GetMock<IDbRoleLocalizationMapper>().Reset();
      _autoMocker.GetMock<IRoleLocalizationRepository>().Reset();

      _autoMocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(true);

      _autoMocker
        .Setup<ICreateRoleLocalizationRequestValidator, bool>(x => x.ValidateAsync(_request, default).Result.IsValid)
        .Returns(true);

      _autoMocker
        .Setup<IDbRoleLocalizationMapper, DbRoleLocalization>(x => x.Map(_request))
        .Returns(_dbRoleLocalization);

      _autoMocker
        .Setup<IRoleLocalizationRepository, Task<Guid?>>(x => x.CreateAsync(_dbRoleLocalization))
        .ReturnsAsync(_dbRoleLocalization.Id);
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenUserIsNotAdminAsync()
    {
      _autoMocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { NotEnoughRightsErrorMessage }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        Times.Once(),
        Times.Never(),
        Times.Never(),
        Times.Never());
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenRoleIdIsNullAsync()
    {
      CreateRoleLocalizationRequest requestWithoutRole = new CreateRoleLocalizationRequest
      {
        RoleId = null
      };

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { RoleIdCantBeEmptyErrorMessage }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(requestWithoutRole));

      Verifiable(
        Times.Once(),
        Times.Never(),
        Times.Never(),
        Times.Never());
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenValidationIsFailedAsync()
    {
      _autoMocker
        .Setup<ICreateRoleLocalizationRequestValidator, bool>(x => x.ValidateAsync(_request, default).Result.IsValid)
        .Returns(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { RequestIsNotCorrectErrorMessage }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Never(),
        Times.Never());
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenRepositoryReturnNullAsync()
    {
      _autoMocker
        .Setup<IRoleLocalizationRepository, Task<Guid?>>(x => x.CreateAsync(_dbRoleLocalization))
        .ReturnsAsync((Guid?)null);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string>()
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Once(),
        Times.Once());
    }

    [Test]
    public async Task ShouldCreateRoleLocalizationSuccessfullyAsync()
    {
      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Body = _dbRoleLocalization.Id
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Once(),
        Times.Once());
    }
  }
}
