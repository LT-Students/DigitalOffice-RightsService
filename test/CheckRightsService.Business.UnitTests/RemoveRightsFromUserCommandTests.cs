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
    public class RemoveRightsFromUserCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private IRemoveRightsFromUserCommand command;
        private Mock<IAccessValidator> accessValidator;

        private Guid userId;
        private List<int> rightIds;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            accessValidator = new Mock<IAccessValidator>();
            command = new RemoveRightsFromUserCommand(repositoryMock.Object, accessValidator.Object);

            userId = Guid.NewGuid();
            rightIds = new List<int>() { 0, 1 };
        }

        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<List<int>>()));

            command.Execute(userId, rightIds);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserIsNotAdmin()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<List<int>>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(userId, rightIds));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<List<int>>()), Times.Never);
        }
    }
}