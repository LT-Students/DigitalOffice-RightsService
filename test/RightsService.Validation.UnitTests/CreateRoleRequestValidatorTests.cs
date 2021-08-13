using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> _validator;
        private AutoMocker _autoMocker;
        private CreateRoleRequest _goodCreateRoleRequest;
        private CreateRoleRequest _noExistingRightsRequest;
        private CreateRoleRequest _emptyRequest;
        private CreateRoleRequest _lessThanZeroRightRequest;
        private List<DbRight> _dbRightsList;
        private int _rightId;
        private DbRight _dbRight;
        private object _right;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autoMocker = new();

            _goodCreateRoleRequest = new CreateRoleRequest
            {
                Name = "Good Name",
                Description = "good description",
                Rights = new List<int> { 1 }
            };

            _noExistingRightsRequest = new CreateRoleRequest
            {
                Name = "No existing rights",
                Description = "Rights don't exist",
                Rights = new List<int> { 2 }
            };

            _emptyRequest = new CreateRoleRequest();

            _lessThanZeroRightRequest = new CreateRoleRequest
            {
                Name = "less than zero right request",
                Rights = new List<int> { -2 }
            };

            _rightId = 1;
            _dbRight = new DbRight { Id = _rightId };
            _dbRightsList = new List<DbRight>() { _dbRight };

            _autoMocker
                .Setup<IRightRepository, List<DbRight>>(x => x.GetRightsList())
                .Returns(_dbRightsList);

            _autoMocker
                .Setup<IMemoryCache, ICacheEntry>(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(_autoMocker.GetMock<ICacheEntry>().Object);

            _validator = new CreateRoleRequestValidator(
                _autoMocker.GetMock<IRightRepository>().Object,
                _autoMocker.GetMock<IRoleRepository>().Object,
                _autoMocker.GetMock<IRoleRightsCompareHelper>().Object,
                _autoMocker.GetMock<IMemoryCache>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _autoMocker
                .Setup<IMemoryCache, bool>(x => x.TryGetValue(It.IsAny<object>(), out _right))
                .Returns(true);

            _autoMocker
                .Setup<IRoleRightsCompareHelper, bool>(x => x.Compare(It.IsAny<List<int>>(), It.IsAny<IRoleRepository>()))
                .Returns(true);
        }

        [Test]
        public void ShouldValidateSuccessfullWhenRightRequestIsValidAndRightInCache()
        {
            _validator.TestValidate(_goodCreateRoleRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldValidateSuccessfullWhenRightRequestIsValidAndRightInDb()
        {
            _autoMocker
                .Setup<IMemoryCache, bool>(x => x.TryGetValue(It.IsAny<object>(), out _right))
                .Returns(false);

            _validator.TestValidate(_goodCreateRoleRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsEmpty()
        {
            _validator.TestValidate(_emptyRequest)
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Role name must not be empty.");
        }

        [Test]
        public void ShouldThrowValidationErrorWhenRightIdIsLessThanZero()
        {
            _validator.TestValidate(_lessThanZeroRightRequest)
                .ShouldHaveValidationErrorFor(x => x.Rights)
                .WithErrorMessage("Right number can not be less than zero.");
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightDoesNotExist()
        {
            _autoMocker
                .Setup<IMemoryCache, bool>(x => x.TryGetValue(It.IsAny<object>(), out _right))
                .Returns(false);

            _validator.TestValidate(_noExistingRightsRequest)
                .ShouldHaveValidationErrorFor(x => x.Rights)
                .WithErrorMessage("Some rights does not exist.");
        }

        [Test]
        public void ShouldHaveValidationErrorWhenRightsSetIsNotUnique()
        {
            _autoMocker
                .Setup<IRoleRightsCompareHelper, bool>(x => x.Compare(It.IsAny<List<int>>(), It.IsAny<IRoleRepository>()))
                .Returns(false);

            _validator.TestValidate(_goodCreateRoleRequest)
                .ShouldHaveValidationErrorFor(x => x.Rights)
                .WithErrorMessage("Set of rights in this role already exists in other role");
        }
    }
}
