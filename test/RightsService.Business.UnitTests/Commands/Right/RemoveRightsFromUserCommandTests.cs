using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Business.Commands.Right;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
  public class RemoveRightsFromUserCommandTests
    {
        private Mock<IRightLocalizationRepository> repositoryMock;
        private Mock<IRightsIdsValidator> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private IRemoveRightsFromUserCommand command;
        private Mock<IAccessValidator> accessValidator;

        private Guid userId;
        private IEnumerable<int> rightsIds;
        private ValidationResult validationResultError;

        //[SetUp]
        //public void SetUp()
        //{
        //    repositoryMock = new Mock<IRightLocalizationRepository>();
        //    accessValidator = new Mock<IAccessValidator>();
        //    validatorMock = new Mock<IRightsIdsValidator>();
        //    command = new RemoveRightsFromUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);

        //    userId = Guid.NewGuid();
        //    rightsIds = new List<int>() { 0, 1 };

        //    validationResultError = new ValidationResult(
        //        new List<ValidationFailure>
        //        {
        //            new ValidationFailure("error", "something", null)
        //        });

        //    validationResultIsValidMock = new Mock<ValidationResult>();

        //    validationResultIsValidMock
        //        .Setup(x => x.IsValid)
        //        .Returns(true);
        //}

        //[Test]
        //public void ShouldRemoveRightsFromUser()
        //{
        //    accessValidator
        //        .Setup(x => x.IsAdmin(null))
        //        .Returns(true);

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultIsValidMock.Object);

        //    repositoryMock
        //        .Setup(x => x.RemoveUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

        //    command.ExecuteAsync(userId, rightsIds);
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenUserIsNotAdmin()
        //{
        //    accessValidator
        //        .Setup(x => x.IsAdmin(null))
        //        .Returns(false);

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultIsValidMock.Object);

        //    repositoryMock
        //        .Setup(x => x.RemoveUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

        //    Assert.Throws<ForbiddenException>(() => command.ExecuteAsync(userId, rightsIds));
        //    repositoryMock.Verify(repository => repository.RemoveUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()), Times.Never);
        //}

        //[Test]
        //public void ShouldThrowExceptionWhenValidatorThrowsException()
        //{
        //    accessValidator
        //        .Setup(x => x.IsAdmin(null))
        //        .Returns(true);

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultError);

        //    Assert.Throws<ValidationException>(() => command.ExecuteAsync(userId, rightsIds));
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //}
    }
}
