using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.User;
using LT.DigitalOffice.RightsService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.User
{
  public class EditUserRoleCommandTests
  {
    private AutoMocker _mocker;
    private IEditUserRoleCommand _command;

    private EditUserRoleRequest _request;
    private EditUserRoleRequest _requestWithNullRoleId;
    private DbUserRole _dbUserRole;
    private DbUserRole _dbUserRoleWithoutChangeRoleId;
    private OperationResultResponse<bool> _goodResponse;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times requestValidatorTimes,
      Times dbUserRoleMapperTimes,
      Times repositoryGetTimes,
      Times repositoryRemoveTimes,
      Times repositoryEditTimes,
      Times repositoryCreateTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x =>
          x.HasRightsAsync(It.IsAny<int>()),
        accessValidatorTimes);

      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(x =>
          x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorTimes);

      _mocker.Verify<IEditUserRoleRequestValidator, Task<ValidationResult>>(x =>
          x.ValidateAsync(It.IsAny<EditUserRoleRequest>(), default),
        requestValidatorTimes);

      _mocker.Verify<IDbUserRoleMapper, DbUserRole>(x =>
          x.Map(It.IsAny<EditUserRoleRequest>()),
        dbUserRoleMapperTimes);

      _mocker.Verify<IUserRoleRepository, Task<DbUserRole>>(x =>
          x.GetAsync(It.IsAny<Guid>()),
        repositoryGetTimes);
      _mocker.Verify<IUserRoleRepository, Task<bool>>(x =>
          x.RemoveAsync(It.IsAny<Guid>(), It.IsAny<DbUserRole>(), It.IsAny<Guid?>()),
        repositoryRemoveTimes);
      _mocker.Verify<IUserRoleRepository, Task<bool>>(x =>
          x.EditAsync(It.IsAny<DbUserRole>(), It.IsAny<Guid>()),
        repositoryEditTimes);
      _mocker.Verify<IUserRoleRepository, Task<Guid?>>(x =>
          x.CreateAsync(It.IsAny<DbUserRole>()),
        repositoryCreateTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<EditUserRoleCommand>();

      _request = new()
      {
        UserId = Guid.NewGuid(),
        RoleId = Guid.NewGuid()
      };

      _requestWithNullRoleId = new()
      {
        UserId = Guid.NewGuid()
      };

      _dbUserRole = new DbUserRole
      {
        Id = Guid.NewGuid(),
        UserId = _request.UserId,
        RoleId = _request.RoleId.Value,
        CreatedBy = Guid.NewGuid(),
        IsActive = true
      };

      _dbUserRoleWithoutChangeRoleId = new DbUserRole
      {
        Id = Guid.NewGuid(),
        UserId = _requestWithNullRoleId.UserId,
        RoleId = Guid.NewGuid(),
        CreatedBy = Guid.NewGuid(),
        IsActive = true
      };

      _goodResponse = new()
      {
        Body = true
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IEditUserRoleRequestValidator>().Reset();
      _mocker.GetMock<IUserRoleRepository>().Reset();
      _mocker.GetMock<IDbUserRoleMapper>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddRemoveUsersRoles))
        .ReturnsAsync(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Not enough rights." }
        });

      _mocker
        .Setup<IEditUserRoleRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<EditUserRoleRequest>(), default))
        .ReturnsAsync(new ValidationResult() { });

      _mocker
        .Setup<IDbUserRoleMapper, DbUserRole>(x => x.Map(It.IsAny<EditUserRoleRequest>()))
        .Returns(_dbUserRole);

      _mocker
        .Setup<IUserRoleRepository, Task<DbUserRole>>(x => x.GetAsync(_request.UserId))
        .ReturnsAsync(_dbUserRole);

      _mocker
        .Setup<IUserRoleRepository, Task<DbUserRole>>(x => x.GetAsync(_request.UserId))
        .ReturnsAsync(_dbUserRoleWithoutChangeRoleId);
    }

    [Test]
    public async Task NotEnoughRightsTest()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false,
        Errors = new List<string> { "Not enough rights." }
      };

      _mocker.Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddRemoveUsersRoles))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        dbUserRoleMapperTimes: Times.Never(),
        requestValidatorTimes: Times.Never(),
        repositoryGetTimes: Times.Never(),
        repositoryRemoveTimes: Times.Never(),
        repositoryEditTimes: Times.Never(),
        repositoryCreateTimes: Times.Never());
    }

    [Test]
    public async Task ValidationFailureTest()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false,
        Errors = new List<string> { "Request is not correct." }
      };

      _mocker
       .Setup<IEditUserRoleRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<EditUserRoleRequest>(), default))
       .ReturnsAsync(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("_", "Error") }));

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
         accessValidatorTimes: Times.Once(),
         responseCreatorTimes: Times.Once(),
         dbUserRoleMapperTimes: Times.Never(),
         requestValidatorTimes: Times.Once(),
         repositoryGetTimes: Times.Never(),
         repositoryRemoveTimes: Times.Never(),
         repositoryEditTimes: Times.Never(),
         repositoryCreateTimes: Times.Never());
    }

    [Test]
    public async Task BadRequestTest()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false,
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_requestWithNullRoleId));

      Verifiable(
         accessValidatorTimes: Times.Once(),
         responseCreatorTimes: Times.Once(),
         dbUserRoleMapperTimes: Times.Never(),
         requestValidatorTimes: Times.Once(),
         repositoryGetTimes: Times.Once(),
         repositoryRemoveTimes: Times.Never(),
         repositoryEditTimes: Times.Never(),
         repositoryCreateTimes: Times.Never());
    }

    [Test]
    public async Task RemoveUserRoleSuccesTest()
    {
      _mocker
       .Setup<IUserRoleRepository, Task<DbUserRole>>(x => x.GetAsync(_requestWithNullRoleId.UserId))
       .ReturnsAsync(_dbUserRoleWithoutChangeRoleId);

      _mocker
        .Setup<IUserRoleRepository, Task<bool>>(x => x.RemoveAsync(_requestWithNullRoleId.UserId, _dbUserRoleWithoutChangeRoleId, null))
        .ReturnsAsync(true);

      SerializerAssert.AreEqual(_goodResponse, await _command.ExecuteAsync(_requestWithNullRoleId));

      Verifiable(
         accessValidatorTimes: Times.Once(),
         responseCreatorTimes: Times.Never(),
         dbUserRoleMapperTimes: Times.Never(),
         requestValidatorTimes: Times.Once(),
         repositoryGetTimes: Times.Once(),
         repositoryRemoveTimes: Times.Once(),
         repositoryEditTimes: Times.Never(),
         repositoryCreateTimes: Times.Never());
    }

    [Test]
    public async Task EditUserRoleSuccesTest()
    {
      _mocker
       .Setup<IUserRoleRepository, Task<DbUserRole>>(x => x.GetAsync(_request.UserId))
       .ReturnsAsync(_dbUserRole);

      _mocker
        .Setup<IUserRoleRepository, Task<bool>>(x => x.EditAsync(_dbUserRole, _request.RoleId.Value))
        .ReturnsAsync(true);

      SerializerAssert.AreEqual(_goodResponse, await _command.ExecuteAsync(_request));

      Verifiable(
         accessValidatorTimes: Times.Once(),
         responseCreatorTimes: Times.Never(),
         dbUserRoleMapperTimes: Times.Never(),
         requestValidatorTimes: Times.Once(),
         repositoryGetTimes: Times.Once(),
         repositoryRemoveTimes: Times.Never(),
         repositoryEditTimes: Times.Once(),
         repositoryCreateTimes: Times.Never());
    }

    [Test]
    public async Task CreateUserRoleSuccesTest()
    {
      _mocker
       .Setup<IUserRoleRepository, Task<DbUserRole>>(x => x.GetAsync(_request.UserId))
       .ReturnsAsync(default(DbUserRole));

      _mocker
        .Setup<IUserRoleRepository, Task<Guid?>>(x => x.CreateAsync(_dbUserRole))
        .ReturnsAsync(_dbUserRole.Id);

      _mocker
        .Setup<IDbUserRoleMapper, DbUserRole>(x => x.Map(_request))
        .Returns(_dbUserRole);

      SerializerAssert.AreEqual(_goodResponse, await _command.ExecuteAsync(_request));

      Verifiable(
         accessValidatorTimes: Times.Once(),
         responseCreatorTimes: Times.Never(),
         dbUserRoleMapperTimes: Times.Once(),
         requestValidatorTimes: Times.Once(),
         repositoryGetTimes: Times.Once(),
         repositoryRemoveTimes: Times.Never(),
         repositoryEditTimes: Times.Never(),
         repositoryCreateTimes: Times.Once());
    }
  }
}
