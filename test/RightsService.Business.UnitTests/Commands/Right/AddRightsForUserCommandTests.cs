using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Business.Commands.Right;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
    public class AddRightsForUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IRightsIdsValidator> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IAccessValidator> accessValidator;
        private IAddRightsForUserCommand command;

        private Guid userId;
        private List<int> rightsIds;
        private ValidationResult validationResultError;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            validatorMock = new Mock<IRightsIdsValidator>();
            accessValidator = new Mock<IAccessValidator>();
            command = new AddRightsForUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);

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
        public void ShouldAddRightsForUser()
        {
            userId = Guid.NewGuid();
            rightsIds = new List<int>() { 0, 1 };

            accessValidator
                .Setup(x => x.IsAdmin(null))
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            command.Execute(userId, rightsIds);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenAccessValidatorThrowFalse()
        {
            userId = Guid.NewGuid();
            rightsIds = new List<int>() { 0, 1 };

            accessValidator
                .Setup(x => x.IsAdmin(null))
                .Returns(false);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(userId, rightsIds));
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenRepositoryThrowException()
        {
            rightsIds = new List<int>() { 1 };

            accessValidator
                .Setup(x => x.IsAdmin(null))
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(userId, rightsIds));
        }

        [Test]
        public void ShouldThrowExceptionWhenValidatorThrowsException()
        {
            accessValidator
                .Setup(x => x.IsAdmin(null))
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(userId, rightsIds));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }
    }
}