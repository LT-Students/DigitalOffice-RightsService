using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Business.Role;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using FluentValidation;
using LT.DigitalOffice.UnitTestKernel;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Role
{
    internal class CreateRoleCommandTests
    {
        private AutoMocker _autoMocker;
        private DbRole _dbRole;
        private CreateRoleRequest _newRequest;
        private ICreateRoleCommand _command;
        private OperationResultResponse<Guid> _goodResponse;
        private IDictionary<object, object> _items;

        private Guid _userId = Guid.NewGuid();
        private Guid _roleId = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autoMocker = new AutoMocker();

            _items = new Dictionary<object, object>();
            _items.Add("UserId", _userId);

            _newRequest = new CreateRoleRequest
            {
                Name = "Name",
                Description = "Description",
                Rights = new List<int> { 1 }
            };

            _dbRole = new DbRole
            {
                Id = _roleId
            };

            _goodResponse = new OperationResultResponse<Guid>
            {
                Body = _dbRole.Id,
                Status = OperationResultStatusType.FullSuccess,
                Errors = new List<string>()
            };

            _autoMocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _autoMocker
                .Setup<IRoleRepository, Guid>(x => x.Create(_dbRole))
                .Returns(_roleId);

            _autoMocker
                .Setup<IDbRoleMapper, DbRole>(x => x.Map(It.IsAny<CreateRoleRequest>(), It.IsAny<Guid>()))
                .Returns(_dbRole);
            _autoMocker
                .Setup<IDbRoleMapper, DbRole>(x => x.Map(It.Is<CreateRoleRequest>(x => x == null), It.IsAny<Guid>()))
                .Throws(new ArgumentNullException("CreateRoleRequest"));

            _command = new CreateRoleCommand(
                _autoMocker.GetMock<IHttpContextAccessor>().Object,
                _autoMocker.GetMock<IRoleRepository>().Object,
                _autoMocker.GetMock<ICreateRoleRequestValidator>().Object,
                _autoMocker.GetMock<IDbRoleMapper>().Object,
                _autoMocker.GetMock<IAccessValidator>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _autoMocker
                .Setup<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _autoMocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenAccessValidatorIsFalse()
        {
            _autoMocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            Assert.Throws<ForbiddenException>(
                () => _command.Execute(_newRequest), "Not enough rights.");
        }

        [Test]
        public void ShouldThrowValidatorException()
        {
            _autoMocker
                .Setup<ICreateRoleRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(
                () => _command.Execute(_newRequest), "CreateRoleRequest");
        }

        [Test]
        public void ShouldThrowHttpContextException()
        {
            _autoMocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Throws(new ArgumentException("HttpContext exception"));

            Assert.Throws<ArgumentException>(
                () => _command.Execute(_newRequest), "HttpContext exception");
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => _command.Execute(null));
        }

        [Test]
        public void SholdThrowNoExceptions()
        {
            OperationResultResponse<Guid> response = _command.Execute(_newRequest);

            SerializerAssert.AreEqual(response, _goodResponse);
        }
    }
}
