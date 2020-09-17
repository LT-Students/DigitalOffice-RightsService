using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsServiceUnitTests.Validators
{
    public class AddRightsForUserValidatorTests
    {
        private IValidator<AddRightsForUserRequest> validator;

        [SetUp]
        public void SetUp()
        {
            validator = new AddRightsForUserValidator();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserIdNull()
        {
            validator.ShouldHaveValidationErrorFor(x => x.UserId, Guid.Empty);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRightsIdIsNull()
        {
            validator.ShouldHaveValidationErrorFor(x => x.RightsIds, null as List<int>);
        }
    }
}