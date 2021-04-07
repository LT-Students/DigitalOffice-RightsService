using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.UnitTests
{
    public class GetRightsListCommandTests
    {
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IRightsMapper> mapperMock;
        private IGetRightsListCommand command;
        private List<DbRight> dbRightsList;
        private DbRight dbRight;
        private Right right;

        [SetUp]
        public void Setup()
        {
            mapperMock = new Mock<IRightsMapper>();
            repositoryMock = new Mock<ICheckRightsRepository>();
            command = new GetRightsListCommand(repositoryMock.Object, mapperMock.Object);
            dbRight = new DbRight { Id = 0, Name = "Right", Description = "Allows you everything" };
            right = new Right {Id = 0, Name = "Right", Description = "Allows you everything"};
            dbRightsList = new List<DbRight> { dbRight };
        }

        [Test]
        public void ShouldGetRightsList()
        {
            repositoryMock.Setup(repository => repository.GetRightsList())
                .Returns(dbRightsList)
                .Verifiable();
            mapperMock.Setup(mapper => mapper.Map(dbRight))
                .Returns(right)
                .Verifiable();

            Assert.That(command.Execute(), Is.EquivalentTo(new List<Right> {right}));
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetRightsList())
                .Throws(new Exception("Bad Request"));

            Assert.That(() => command.Execute(), Throws.TypeOf<Exception>().And.Message.EqualTo("Bad Request"));
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetRightsList())
                .Returns(dbRightsList)
                .Verifiable();
            mapperMock.Setup(mapper => mapper.Map(It.IsAny<DbRight>()))
                .Throws(new Exception("Bad Request"))
                .Verifiable();

            Assert.That(() => command.Execute(), Throws.TypeOf<Exception>().And.Message.EqualTo("Bad Request"));
            repositoryMock.Verify();
            mapperMock.Verify();
        }
    }
}