using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business.Role
{
    /// <inheritdoc/>
    public class CreateRoleCommand : ICreateRoleCommand
    {
        private readonly IRoleRepository _repository;
        private readonly ILogger<CreateRoleCommand> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICreateRoleRequestValidator _validator;
        private readonly IDbRoleMapper _mapper;
        private readonly IAccessValidator _accessValidator;

        public CreateRoleCommand(
            ILogger<CreateRoleCommand> logger,
            IHttpContextAccessor httpContextAccessor,
            IRoleRepository repository,
            ICreateRoleRequestValidator validator,
            IDbRoleMapper mapper,
            IAccessValidator accessValidator)
        {
            _logger = logger;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _mapper = mapper;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public OperationResultResponse<Guid> Execute(CreateRoleRequest request)
        {
            if (!_accessValidator.IsAdmin())
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            List<string> errors = new();

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var roleId = _repository.Create(_mapper.Map(request, userId));

            return new OperationResultResponse<Guid>
            {
                Body = roleId,
                Status = errors.Any()
                    ? OperationResultStatusType.PartialSuccess
                    : OperationResultStatusType.FullSuccess,
                Errors = errors
            };
        }
    }
}