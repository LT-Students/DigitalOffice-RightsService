using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

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
        private readonly IMemoryCache _memoryCache;

        private bool CheckNameUniqueness(string name, out List<string> roleName)
        {
            List<string> names = _memoryCache.Get<List<string>>(CacheKeys.RoleNames);

            if (names == null)
            {
                names = _repository.GetNames();

                _memoryCache.Set(CacheKeys.RoleNames, names);
            }

            roleName = names;

            return !names.Contains(name);
        }

        public CreateRoleCommand(
            IHttpContextAccessor httpContextAccessor,
            IRoleRepository repository,
            ICreateRoleRequestValidator validator,
            IDbRoleMapper mapper,
            IAccessValidator accessValidator,
            IMemoryCache memoryCache)
        {
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _mapper = mapper;
            _accessValidator = accessValidator;
            _memoryCache = memoryCache;
        }

        public OperationResultResponse<Guid> Execute(CreateRoleRequest request)
        {
            if (!_accessValidator.IsAdmin())
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            if (!CheckNameUniqueness(request.Name, out List<string> roleNames))
            {
                return new OperationResultResponse<Guid>
                {
                    Status = OperationResultStatusType.Conflict,
                    Errors = new() { $"Role with name: '{request.Name}' already exists." }
                };
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var roleId = _repository.Create(_mapper.Map(request, userId));

            roleNames.Add(request.Name);

            return new OperationResultResponse<Guid>
            {
                Body = roleId,
                Status = OperationResultStatusType.FullSuccess
            };
        }
    }
}