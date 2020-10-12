using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
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
        private Mock<IAccessValidator> accessValidator;
        private IAddRightsForUserCommand command;
        private Guid userId;
        private List<int> rightsIds;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            accessValidator = new Mock<IAccessValidator>();
            command = new AddRightsForUserCommand(repositoryMock.Object, accessValidator.Object);
        }

        [Test]
        public void ShouldAddRightsForUser()
        {
            userId = Guid.NewGuid();
            rightsIds = new List<int>() { 0, 1 };

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

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
                .Setup(x => x.IsAdmin())
                .Returns(false);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()));

            Assert.Throws<ForbiddenException>(() => command.Execute(userId, rightsIds));
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenRepositoryThrowException()
        {
            rightsIds = new List<int>() { 1 };

            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.AddRightsToUser(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(userId, rightsIds));
        }
    }
}