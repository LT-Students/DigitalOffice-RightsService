using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.CheckRightsService.Validation
{
    public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>
    {
        private readonly IMemoryCache cache;
        private readonly ICheckRightsRepository repository;

        public RightsIdsValidator(
            [FromServices] ICheckRightsRepository repository,
            [FromServices] IMemoryCache cache)
        {
            this.repository = repository;
            this.cache = cache;

            RuleFor(rightsIds => rightsIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rights list can not be empty")
                .Must(rightsIds =>
                {
                    return rightsIds.All(r => r > 0);
                }).WithMessage("Right number can not be less than zero.")
                .Must(rightsIds => DoesRightsExist(rightsIds)).WithMessage("Some rights does not exist.");
        }

        private bool DoesRightsExist(IEnumerable<int> rightsIds)
        {
            var dbRights = new List<DbRight>();

            foreach (int rightId in rightsIds)
            {
                if (!cache.TryGetValue(rightId, out object right))
                {
                    if (!dbRights.Any())
                    {
                        dbRights = repository.GetRightsList();
                    }

                    var dbRight = dbRights.Find(right => right.Id == rightId);

                    if (dbRight == null)
                    {
                        return false;
                    }

                    cache.Set(rightId, dbRight);
                }
            }

            return true;
        }
    }
}
