using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.UnitTests
{
    public class RightsIdsValidatorTests
    {
        private IValidator<IEnumerable<int>> validator;
        private List<DbRight> existingRightsList;
        private Mock<IRightRepository> repositoryMock;
        private Mock<IMemoryCache> cacheMock;

        private int rightId;
        private DbRight dbRight;
        private object right;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IRightRepository>();
            cacheMock = new Mock<IMemoryCache>();

            rightId = 1;
            dbRight = new DbRight { Id = rightId };
            existingRightsList = new List<DbRight>() { dbRight };

            repositoryMock
                .Setup(r => r.GetRightsList())
                .Returns(existingRightsList);

            validator = new RightsIdsValidator(repositoryMock.Object, cacheMock.Object);
        }

        //[Test]
        //public void ShouldThrowValidationExceptionWhenRightListIsEmpty()
        //{
        //    validator.ShouldHaveValidationErrorFor(x => x, new List<int>());
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenRightIdIsInvalid()
        //{
        //    repositoryMock
        //        .Setup(x => x.GetRightsList())
        //        .Returns(existingRightsList);

        //    validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { -1 });
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenRightDoesNotExist()
        //{
        //    cacheMock
        //        .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
        //        .Returns(false);

        //    repositoryMock
        //        .Setup(x => x.GetRightsList())
        //        .Returns(existingRightsList);

        //    validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { 10000 });
        //}

        //[Test]
        //public void ShouldValidateSuccessfullyWhenRightsListExistInDb()
        //{
        //    cacheMock
        //        .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
        //        .Returns(false);

        //    repositoryMock
        //        .Setup(x => x.GetRightsList())
        //        .Returns(existingRightsList);

        //    cacheMock
        //        .Setup(x => x.CreateEntry(It.IsAny<object>()))
        //        .Returns(Mock.Of<ICacheEntry>());

        //    validator.ShouldNotHaveValidationErrorFor(x => x, new List<int>() { rightId });
        //}

        //[Test]
        //public void ShouldValidateSuccessfullyWhenRightsListExistInCache()
        //{
        //    cacheMock
        //        .Setup(x => x.TryGetValue(It.IsAny<object>(), out right))
        //        .Returns(true);

        //    validator.ShouldNotHaveValidationErrorFor(x => x, new List<int>() { rightId });
        //}
    }
}
