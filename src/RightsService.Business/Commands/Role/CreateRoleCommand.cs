using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Business.Role
{
  /// <inheritdoc/>
  public class CreateRoleCommand : ICreateRoleCommand
  {
    private readonly IRoleRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICreateRoleRequestValidator _validator;
    private readonly IDbRoleMapper _mapper;
    private readonly IAccessValidator _accessValidator;

    public CreateRoleCommand(
      IHttpContextAccessor httpContextAccessor,
      IRoleRepository repository,
      ICreateRoleRequestValidator validator,
      IDbRoleMapper mapper,
      IAccessValidator accessValidator)
    {
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateRoleRequest request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<Guid>
      {
        Body = await _repository.CreateAsync(_mapper.Map(request)),
        Status = OperationResultStatusType.FullSuccess
      };
    }
  }
}
