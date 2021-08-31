using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.RightsService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> validator;
        private Mock<IRightsIdsValidator> rightIdsValidator;

        [SetUp]
        public void SetUp()
        {
            rightIdsValidator = new();

            rightIdsValidator
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            validator = new CreateRoleRequestValidator(rightIdsValidator.Object);
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
