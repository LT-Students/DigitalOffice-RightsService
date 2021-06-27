using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business.Commands.Right
{
    /// <inheritdoc cref="IGetRightsListCommand"/>
    public class GetRightsListCommand : IGetRightsListCommand
    {
        private readonly ICheckRightsRepository _repository;
        private readonly IRightMapper _mapper;

        public GetRightsListCommand(
            ICheckRightsRepository repository,
            IRightMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<RightResponse> Execute()
        {
            return _repository.GetRightsList().Select(right => _mapper.Map(right)).ToList();
        }
    }
}
