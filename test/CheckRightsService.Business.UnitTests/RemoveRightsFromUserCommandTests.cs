﻿using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
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
        private IEnumerable<int> rightsIds;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            accessValidator = new Mock<IAccessValidator>();
            command = new RemoveRightsFromUserCommand(repositoryMock.Object, accessValidator.Object);

            userId = Guid.NewGuid();
            rightsIds = new List<int>() { 0, 1 };
        }

        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

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

            repositoryMock
                .Setup(x => x.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(userId, rightsIds));
            repositoryMock.Verify(repository => repository.RemoveRightsFromUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()), Times.Never);
        }
    }
}