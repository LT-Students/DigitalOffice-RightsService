using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.RightsService.Mappers.UnitTests
{
  public class RightsMapperTests
    {
        private IRightInfoMapper mapper;

        private const int Id = 0;
        private const string Name = "Right";
        private const string Description = "Allows you everything";

        private DbRightsLocalization dbRight;

        //[SetUp]
        //public void SetUp()
        //{
        //    mapper = new RightInfoMapper();
        //    dbRight = new DbRightsLocalization
        //    {
        //        Id = Id,
        //        Name = Name,
        //        Description = Description
        //    };
        //}

        //#region DbRight to Right
        //[Test]
        //public void ShouldThrowExceptionWhenArgumentIsNull()
        //{
        //    Assert.Throws<ArgumentNullException>(() => mapper.Map(null));
        //}

        //[Test]
        //public void ShouldReturnRightModel()
        //{
        //    var result = mapper.Map(dbRight);

        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<RightInfo>(result);
        //    Assert.AreEqual(Id, result.Id);
        //    Assert.AreEqual(Name, result.Name);
        //    Assert.AreEqual(Description, result.Description);
        //}
        //#endregion
    }
}
