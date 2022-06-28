using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Business.Commands.User
{
  public class EditUserRoleCommand : IEditUserRoleCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IEditUserRoleRequestValidator _validator;
    private readonly IDbUserRoleMapper _mapper;
    private readonly IUserRoleRepository _repository;

    public EditUserRoleCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IEditUserRoleRequestValidator validator,
      IDbUserRoleMapper mapper,
      IUserRoleRepository repository)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditUserRoleRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddRemoveUsersRoles))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      DbUserRole oldUser = await _repository.GetAsync(request.UserId);

      if (oldUser is null && !request.RoleId.HasValue)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      if (!request.RoleId.HasValue)
      {
        response.Body = await _repository.RemoveAsync(request.UserId, oldUser);
      }
      else
      {
        response.Body = oldUser is not null
          ? await _repository.EditAsync(oldUser, request.RoleId.Value)
          : (await _repository.CreateAsync(_mapper.Map(request))).HasValue;
      }

      return response;
    }
  }
}
