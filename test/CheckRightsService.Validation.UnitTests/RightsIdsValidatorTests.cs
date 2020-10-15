using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Validation.UnitTests
{
    public class RightsIdsValidatorTests
    {
        private IValidator<IEnumerable<int>> validator;
        private List<DbRight> existingRightsList;
        private Mock<ICheckRightsRepository> repositoryMock;
        private Mock<IMemoryCache> cacheMock;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<ICheckRightsRepository>();
            cacheMock = new Mock<IMemoryCache>();
            validator = new RightsIdsValidator(repositoryMock.Object, cacheMock.Object);

            existingRightsList = new List<DbRight>() { new DbRight{ Id = 1 } };
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightListIsEmpty()
        {
            validator.ShouldHaveValidationErrorFor(x => x, new List<int>());
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightIdIsInvalid()
        {
            repositoryMock
                .Setup(x => x.GetRightsList())
                .Returns(existingRightsList);

            validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { -1 });
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightDoesNotExist()
        {
            repositoryMock
                .Setup(x => x.GetRightsList())
                .Returns(existingRightsList);

            validator.ShouldHaveValidationErrorFor(x => x, new List<int>() { 10000 });
        }

        [Test]
        public void ShouldValidateSuccessfullyWhenRightsListExist()
        {
            repositoryMock
                .Setup(x => x.GetRightsList())
                .Returns(existingRightsList);

            validator.ShouldNotHaveValidationErrorFor(x => x, new List<int>() { 1 });
        }
    }
}
