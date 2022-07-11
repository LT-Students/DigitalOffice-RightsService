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
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.RoleLocalization
{
  public class EditRoleLocalizationCommandTests
  {
    private const string RequestIsNotCorrectErrorMessage = "Request is not correct.";
    private const string NotEnoughRightsErrorMessage = "Not enough rights.";

    private AutoMocker _autoMocker;
    private IEditRoleLocalizationCommand _command;

    private JsonPatchDocument<EditRoleLocalizationRequest> _request;
    private Guid _roleLocalizationId;
    private (Guid, JsonPatchDocument<EditRoleLocalizationRequest>) _tuple;
    private JsonPatchDocument<DbRoleLocalization> _dbRoleLocalization;

    private void Verifiable(
      Times accessValidatorTimes,
      Times requestValidatorTimes,
      Times mapperTimes,
      Times repositoryTimes)
    {
      _autoMocker.Verify<IAccessValidator>(
        x => x.IsAdminAsync(It.IsAny<Guid?>()),
        accessValidatorTimes);

      _autoMocker.Verify<IEditRoleLocalizationRequestValidator>(
        x => x.ValidateAsync(_tuple, It.IsAny<CancellationToken>()),
        requestValidatorTimes);

      _autoMocker.Verify<IPatchDbRoleLocalizationMapper>(
        x => x.Map(_request),
        mapperTimes);

      _autoMocker.Verify<IRoleLocalizationRepository>(
        x => x.EditRoleLocalizationAsync(_roleLocalizationId, _dbRoleLocalization),
        repositoryTimes);

      _autoMocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _command = _autoMocker.CreateInstance<EditRoleLocalizationCommand>();

      //TODO: Not sure there is a point to fill it with data. We mock it anyway
      _request = new JsonPatchDocument<EditRoleLocalizationRequest>(
        new List<Operation<EditRoleLocalizationRequest>>(),
        new CamelCasePropertyNamesContractResolver());

      _dbRoleLocalization = new();
      _roleLocalizationId = Guid.NewGuid();
      _tuple = (_roleLocalizationId, _request);

      _autoMocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(200);

      _autoMocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(
          x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>
        {
          Errors = new() { RequestIsNotCorrectErrorMessage }
        });

      _autoMocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(
          x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>
        {
          Errors = new() { NotEnoughRightsErrorMessage }
        });
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IAccessValidator>().Reset();
      _autoMocker.GetMock<IEditRoleLocalizationRequestValidator>().Reset();
      _autoMocker.GetMock<IPatchDbRoleLocalizationMapper>().Reset();
      _autoMocker.GetMock<IRoleLocalizationRepository>().Reset();

      _autoMocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(true);

      _autoMocker
        .Setup<IEditRoleLocalizationRequestValidator, bool>(x => x.ValidateAsync(_tuple, default).Result.IsValid)
        .Returns(true);

      _autoMocker
        .Setup<IPatchDbRoleLocalizationMapper, JsonPatchDocument<DbRoleLocalization>>(x => x.Map(_request))
        .Returns(_dbRoleLocalization);

      _autoMocker
        .Setup<IRoleLocalizationRepository, Task<bool>>(x => x.EditRoleLocalizationAsync(_roleLocalizationId, _dbRoleLocalization))
        .ReturnsAsync(true);
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenUserIsNotAdminAsync()
    {
      _autoMocker
        .Setup<IAccessValidator, Task<bool>>(x => x.IsAdminAsync(It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { NotEnoughRightsErrorMessage }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_roleLocalizationId, _request));

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
        .Setup<IEditRoleLocalizationRequestValidator, bool>(x => x.ValidateAsync(_tuple, default).Result.IsValid)
        .Returns(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { RequestIsNotCorrectErrorMessage }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_roleLocalizationId, _request));

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Never(),
        Times.Never());
    }

    [Test]
    public async Task ShouldReturnFalseBodyWhenRepositoryReturnsFalseAsync()
    {
      _autoMocker
        .Setup<IRoleLocalizationRepository, Task<bool>>(x => x.EditRoleLocalizationAsync(_roleLocalizationId, _dbRoleLocalization))
        .ReturnsAsync(false);

      Assert.IsFalse((await _command.ExecuteAsync(_roleLocalizationId, _request)).Body);

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Once(),
        Times.Once());
    }

    [Test]
    public async Task ShouldCreateRoleLocalizationSuccessfullyAsync()
    {
      Assert.IsTrue((await _command.ExecuteAsync(_roleLocalizationId, _request)).Body);

      Verifiable(
        Times.Once(),
        Times.Once(),
        Times.Once(),
        Times.Once());
    }
  }
}
