using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business
{
    /// <inheritdoc cref="IGetRightsListCommand"/>
    public class GetRightsListCommand : IGetRightsListCommand
    {
        private readonly ICheckRightsRepository _repository;
        private readonly IRightsMapper _mapper;

        public GetRightsListCommand(
            ICheckRightsRepository repository,
            IRightsMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<Right> Execute()
        {
            return _repository.GetRightsList().Select(right => _mapper.Map(right)).ToList();
        }
    }
}
