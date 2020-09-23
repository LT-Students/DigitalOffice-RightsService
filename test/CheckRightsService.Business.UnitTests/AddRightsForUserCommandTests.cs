using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.UnitTests
{
    public class AddRightsForUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IValidator<AddRightsForUserRequest>> validatorMock;
        private Mock<IAccessValidator> accessValidator;
        private IAddRightsForUserCommand command;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            validatorMock = new Mock<IValidator<AddRightsForUserRequest>>();
            accessValidator = new Mock<IAccessValidator>();
            command = new AddRightsForUserCommand(repositoryMock.Object, validatorMock.Object, accessValidator.Object);
        }

        [Test]
        public void ShouldAddRightsForUser()
        {
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()));

            command.Execute(request);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenAccessValidatorThrowFalse()
        {
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldThrowValidationExceptionWheValidatorThrowException()
        {
            var request = new AddRightsForUserRequest
            {
                RightsIds = new List<int>() { 0, 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<AddRightsForUserRequest>()))
                .Returns(new ValidationResult(
                    new List<ValidationFailure>
                    {
                        new ValidationFailure("test", "something", null)
                    }));

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            repositoryMock.Verify(repository => repository.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()), Times.Never);
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenRepositoryThrowException()
        {
            var request = new AddRightsForUserRequest
            {
                RightsIds = new List<int>() { 1 }
            };

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<AddRightsForUserRequest>()))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(request));
        }
    }
}