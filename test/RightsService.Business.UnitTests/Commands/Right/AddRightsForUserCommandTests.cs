using System;
using System.Collections.Generic;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Moq;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
  public class AddRightsForUserCommandTests
    {
        private Mock<IRightLocalizationRepository> repositoryMock;
        private Mock<IRightsIdsValidator> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IAccessValidator> accessValidator;
        private ICreateUserRightsCommand command;

        private Guid userId;
        private List<int> rightsIds;
        private ValidationResult validationResultError;

        //[SetUp]
        //public void Setup()
        //{
        //    repositoryMock = new Mock<IRightLocalizationRepository>();
        //    validatorMock = new Mock<IRightsIdsValidator>();
        //    accessValidator = new Mock<IAccessValidator>();
        //    command = new AddRightsForUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);

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
        //public void ShouldAddRightsForUser()
        //{
        //    userId = Guid.NewGuid();
        //    rightsIds = new List<int>() { 0, 1 };

        //    accessValidator
        //        .Setup(x => x.IsAdminAsync(null))
        //        .Returns(Task.FromResult(true));

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultIsValidMock.Object);

        //    repositoryMock
        //        .Setup(x => x.AddUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

        //    command.ExecuteAsync(userId, rightsIds);
        //}

        //[Test]
        //public void ShouldThrowForbiddenExceptionWhenAccessValidatorThrowFalse()
        //{
        //    userId = Guid.NewGuid();
        //    rightsIds = new List<int>() { 0, 1 };

        //    accessValidator
        //        .Setup(x => x.IsAdmin(null))
        //        .Returns(false);

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultIsValidMock.Object);

        //    repositoryMock
        //        .Setup(x => x.AddUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

        //    Assert.Throws<ForbiddenException>(() => command.ExecuteAsync(userId, rightsIds));
        //}

        //[Test]
        //public void ShouldThrowBadRequestExceptionWhenRepositoryThrowException()
        //{
        //    rightsIds = new List<int>() { 1 };

        //    accessValidator
        //        .Setup(x => x.IsAdmin(null))
        //        .Returns(true);

        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultIsValidMock.Object);

        //    repositoryMock
        //        .Setup(x => x.AddUserRightsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()))
        //        .Throws(new BadRequestException());

        //    Assert.Throws<BadRequestException>(() => command.ExecuteAsync(userId, rightsIds));
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
