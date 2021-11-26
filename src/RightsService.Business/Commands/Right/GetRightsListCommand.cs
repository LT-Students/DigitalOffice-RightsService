using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Business.Commands.Right
{
  /// <inheritdoc cref="IGetRightsListCommand"/>
  public class GetRightsListCommand : IGetRightsListCommand
  {
    private readonly IRightLocalizationRepository _repository;
    private readonly IRightInfoMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;

    public GetRightsListCommand(
      IRightLocalizationRepository repository,
      IRightInfoMapper mapper,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<List<RightInfo>>> ExecuteAsync(string locale)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<List<RightInfo>>(HttpStatusCode.Forbidden);
      }

      return new()
      {
        Status = Kernel.Enums.OperationResultStatusType.FullSuccess,
        Body = (await _repository.GetRightsListAsync(locale))?.Select(right => _mapper.Map(right)).ToList()
      };
    }
  }
}
