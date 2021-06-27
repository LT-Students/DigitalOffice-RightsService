using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.RightsService.Business.Commands;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Business.Role;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Role
{
    internal class CreateRoleCommandTests
    {
        private AutoMocker _mocker;
        private DbRole _dbRole;
        private CreateRoleRequest _newRequest;
        private ICreateRoleCommand _command;
        private OperationResultResponse<Guid> _response;

        private readonly Guid _authorId = Guid.NewGuid();
        private readonly Guid _roleId = Guid.NewGuid();
        private readonly int _rightId = 123;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<CreateRoleCommand>();

            _newRequest = new CreateRoleRequest
            {
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                Rights = new List<int> { _rightId },
                Users = new List<Guid> { _userId }
            };

            var dbUserRole = new DbUserRole {
                Id = Guid.NewGuid(),
                User = new DbUser { Id = _userId },
                Role = new DbRole { Id = _roleId }
            };
            var dbRoleRight = new DbRoleRight {
                Id = Guid.NewGuid(),
                Role = new DbRole { Id = _roleId },
                Right = new DbRight { Id = _rightId }
            };


            _dbRole = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _authorId,
                Rights = new List<DbRoleRight> { dbRoleRight },
                Users = new List<DbUserRole> { dbUserRole }
            };

            _response = new OperationResultResponse<Guid>
            {
                Body = _dbRole.Id,
                Status = OperationResultStatusType.FullSuccess,
                Errors = new List<string>()
            };
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ICreateRoleRequestValidator>().Reset();
            _mocker.GetMock<IAccessValidator>().Reset();
            _mocker.GetMock<IDbRoleMapper>().Reset();
            _mocker.GetMock<IRoleRepository>().Reset();
            _mocker.GetMock<IHttpContextAccessor>().Reset();
            _mocker.GetMock<IDataProvider>().Reset();
        }

        [Test]
        public void ShouldThrowExceptionWhenCreatingNewUserIsNotAdmin()
        {
            _mocker
              .Setup<IAccessValidator, bool>(x => x.IsAdmin(_authorId))
              .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_newRequest));

            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(_authorId), Times.Once);
            _mocker.Verify<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenCreatingNewRoleWithIncorrectData()
        {
            _mocker
              .Setup<IAccessValidator, bool>(x => x.IsAdmin(_authorId))
              .Returns(true);

            _mocker
                .Setup<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));

            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(_authorId), Times.Once);
            _mocker.Verify<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewRole()
        {
            _mocker
              .Setup<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
              .Returns(true);

            _mocker
                .Setup<IDbRoleMapper, DbRole>(x => x.Map(_newRequest, _authorId))
                .Returns(_dbRole);

            _mocker
                .Setup<IRoleRepository, Guid>(x => x.Create(_dbRole))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));

            _mocker.Verify<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
            _mocker.Verify<IDbRoleMapper, DbRole>(x => x.Map(_newRequest, _authorId), Times.Once);
            _mocker.Verify<IRoleRepository, Guid>(x => x.Create(_dbRole), Times.Once);
        }
    }
}
