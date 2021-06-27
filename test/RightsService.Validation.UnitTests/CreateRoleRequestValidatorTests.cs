using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.UnitTests
{
    public class CreateRoleRequestValidatorTests
    {
        private IValidator<CreateRoleRequest> validator;


        [SetUp]
        public void SetUp()
        {
            validator = new CreateRoleRequestValidator();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooShort()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenNameIsTooLong()
        {
            validator.ShouldHaveValidationErrorFor(x => x.Name, string.Empty.PadRight(200));
        }


    }
}
