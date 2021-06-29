using LT.DigitalOffice.RightsService.Business.Commands.Right;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
    public class GetRightsListCommandTests
    {
        private Mock<IRightRepository> repositoryMock;
        private Mock<IRightMapper> mapperMock;
        private IGetRightsListCommand command;
        private List<DbRight> dbRightsList;
        private DbRight dbRight;
        private RightResponse right;

        [SetUp]
        public void Setup()
        {
            mapperMock = new Mock<IRightMapper>();
            repositoryMock = new Mock<IRightRepository>();
            command = new GetRightsListCommand(repositoryMock.Object, mapperMock.Object);
            dbRight = new DbRight { Id = 0, Name = "Right", Description = "Allows you everything" };
            right = new RightResponse { Id = 0, Name = "Right", Description = "Allows you everything"};
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

            Assert.That(command.Execute(), Is.EquivalentTo(new List<RightResponse> {right}));
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