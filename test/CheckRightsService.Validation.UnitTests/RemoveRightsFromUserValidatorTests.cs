using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Validation.UnitTests
{
    public class RemoveRightsFromUserValidatorTests
    {
        private IValidator<RemoveRightsFromUserRequest> validator;

        [SetUp]
        public void SetUp()
        {
            validator = new RemoveRightsFromUserValidator();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserIdNull()
        {
            validator.ShouldHaveValidationErrorFor(x => x.UserId, Guid.Empty);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightsIdIsNull()
        {
            validator.ShouldHaveValidationErrorFor(x => x.RightIds, null as List<int>);
        }
    }
}