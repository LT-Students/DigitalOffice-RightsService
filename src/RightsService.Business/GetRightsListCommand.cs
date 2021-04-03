using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business
{
    /// <inheritdoc cref="IGetRightsListCommand"/>
    public class GetRightsListCommand : IGetRightsListCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IMapper<DbRight, Right> mapper;

        public GetRightsListCommand(
            ICheckRightsRepository repository,
            IMapper<DbRight, Right> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public List<Right> Execute()
        {
            return repository.GetRightsList().Select(right => mapper.Map(right)).ToList();
        }
    }
}