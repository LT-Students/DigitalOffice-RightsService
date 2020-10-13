using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Validation.UnitTests
{
    public class RightsIdsValidatorTests
    {
        private IValidator<IEnumerable<int>> validator;
        private Mock<ICheckRightsRepository> repositoryMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            validator = new RightsIdsValidator(repositoryMock.Object);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightListIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x, new List<int>());
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightIdIsInvalid()
        {
            validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { -1 });
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightDoesNotExist()
        {
            repositoryMock
                .Setup(x => x.DoesRightExist(It.IsAny<int>()))
                .Returns(false);

            validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { 10000 });
        }

        [Test]
        public void ShouldValidateSuccessfullyWhenRightsList()
        {
            repositoryMock
                .Setup(x => x.DoesRightExist(It.IsAny<int>()))
                .Returns(true);

            validator.ShouldNotHaveValidationErrorFor(x => x, new List<int>() { 1, 2 });
        }
    }
}
