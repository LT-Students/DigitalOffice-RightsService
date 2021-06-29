using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> validator;
        private Mock<IRightRepository> repositoryMock;
        private Mock<IMemoryCache> cacheMock;

        private CreateRoleRequest _createRoleRequest;
        private List<DbRight> existingRightsList;
        private int rightId;
        private DbRight dbRight;
        private object right;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IRightRepository>();
            cacheMock = new Mock<IMemoryCache>();
            validator = new CreateRoleRequestValidator(repositoryMock.Object, cacheMock.Object);

            _createRoleRequest = new CreateRoleRequest
            {
                Name = "test name",
                Description = "test description",
                Rights = new List<int> { 1 }
            };

            rightId = 1;
            dbRight = new DbRight { Id = rightId };
            existingRightsList = new List<DbRight>() { dbRight };
        }

        [Test]
        public void ShouldValidateSuccessfullWhenRightRequestIsValidAndRightInCache()
        {
            cacheMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
                .Returns(true);

            validator.TestValidate(_createRoleRequest).ShouldNotHaveAnyValidationErrors();
        }

        // TODO mock cache set
        //[Test]
        //public void ShouldValidateSuccessfullWhenRightRequestIsValidAndRightInDb()
        //{
        //    cacheMock
        //        .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
        //        .Returns(false);

        //    repositoryMock
        //        .Setup(x => x.GetRightsList())
        //        .Returns(existingRightsList);

        //    validator.TestValidate(_createRoleRequest).ShouldHaveAnyValidationError();
        //}

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooShort()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, string.Empty.PadRight(200));
        }

        // TODO mock cache set
        //[Test]
        //public void ShouldThrowValidationExceptionWhenRightDoesNotExist()
        //{
        //    cacheMock
        //        .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
        //        .Returns(false);

        //    repositoryMock
        //        .Setup(x => x.GetRightsList())
        //        .Returns(existingRightsList);

        //    validator.TestValidate(_createRoleRequest).ShouldHaveAnyValidationError();
        //}
    }
}
