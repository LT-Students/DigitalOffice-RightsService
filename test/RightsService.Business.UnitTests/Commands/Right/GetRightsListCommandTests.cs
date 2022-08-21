using System.Collections.Generic;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using Moq;

namespace LT.DigitalOffice.RightsService.Business.UnitTests.Commands.Right
{
  public class GetRightsListCommandTests
  {
    private Mock<IRightLocalizationRepository> repositoryMock;
    private Mock<IRightInfoMapper> mapperMock;
    private IGetRightsListCommand command;
    private List<DbRightLocalization> dbRightsList;
    private DbRightLocalization dbRight;
    private RightInfo right;

    private const string Locale = "en";

    //[SetUp]
    //public void Setup()
    //{
    //  mapperMock = new Mock<IRightInfoMapper>();
    //  repositoryMock = new Mock<IRightLocalizationRepository>();
    //  command = new GetRightsListCommand(repositoryMock.Object, mapperMock.Object);
    //  dbRight = new DbRightsLocalization { RightId = 0, Locale = Locale, Name = "Right", Description = "Allows you everything" };
    //  right = new RightInfo { RightId = 0, Name = "Right", Description = "Allows you everything" };
    //  dbRightsList = new List<DbRightsLocalization> { dbRight };
    //}

    //[Test]
    //public void ShouldGetRightsList()
    //{
    //  repositoryMock.Setup(repository => repository.GetRightsListAsync(Locale))
    //    .Returns(Task.FromResult(dbRightsList))
    //    .Verifiable();
    //  mapperMock.Setup(mapper => mapper.Map(dbRight))
    //    .Returns(right)
    //    .Verifiable();

    //  Assert.That(command.ExecuteAsync(Locale), Is.EquivalentTo(new List<RightInfo> { right }));
    //  repositoryMock.Verify();
    //  mapperMock.Verify();
    //}

    //[Test]
    //public void ShouldThrowExceptionWhenRepositoryThrowsException()
    //{
    //  repositoryMock.Setup(repository => repository.GetRightsListAsync(Locale))
    //    .Throws(new Exception("Bad Request"));

    //  Assert.That(() => command.ExecuteAsync(Locale), Throws.TypeOf<Exception>().And.Message.EqualTo("Bad Request"));
    //}

    //[Test]
    //public void ShouldThrowExceptionWhenMapperThrowsException()
    //{
    //  repositoryMock.Setup(repository => repository.GetRightsListAsync(Locale))
    //    .Returns(dbRightsList)
    //    .Verifiable();
    //  mapperMock.Setup(mapper => mapper.Map(It.IsAny<DbRightsLocalization>()))
    //    .Throws(new Exception("Bad Request"))
    //    .Verifiable();

    //  Assert.That(() => command.ExecuteAsync(Locale), Throws.TypeOf<Exception>().And.Message.EqualTo("Bad Request"));
    //  repositoryMock.Verify();
    //  mapperMock.Verify();
    //}
  }
}
