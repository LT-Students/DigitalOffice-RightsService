using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business.Commands.Right
{
  /// <inheritdoc cref="IGetRightsListCommand"/>
  public class GetRightsListCommand : IGetRightsListCommand
    {
        private readonly IRightRepository _repository;
        private readonly IRightInfoMapper _mapper;

        public GetRightsListCommand(
            IRightRepository repository,
            IRightInfoMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<RightInfo> Execute(string locale)
        {
            return _repository.GetRightsList(locale).Select(right => _mapper.Map(right)).ToList();
        }
    }
}
