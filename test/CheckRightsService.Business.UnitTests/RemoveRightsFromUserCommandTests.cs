using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.UnitTests
{
    public class RemoveRightsFromUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IValidator<IEnumerable<int>>> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private IRemoveRightsFromUserCommand command;
        private Mock<IAccessValidator> accessValidator;

        private Guid userId;
        private IEnumerable<int> rightsIds;
        private ValidationResult validationResultError;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            accessValidator = new Mock<IAccessValidator>();
            validatorMock = new Mock<IValidator<IEnumerable<int>>>();
            command = new RemoveRightsFromUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);

            userId = Guid.NewGuid();
            rightsIds = new List<int>() { 0, 1 };

            validationResultError = new ValidationResult(
                new List<ValidationFailure>
                {
                    new ValidationFailure("error", "something", null)
                });

            validationResultIsValidMock = new Mock<ValidationResult>();

            validationResultIsValidMock
                .Setup(x => x.IsValid)
                .Returns(true);
        }

        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            command.Execute(userId, rightsIds);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserIsNotAdmin()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(userId, rightsIds));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenValidatorThrowsException()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(userId, rightsIds));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }
    }
}