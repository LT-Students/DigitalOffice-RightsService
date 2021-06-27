using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.Role
{
    public class FindRolesCommand : IFindRolesCommand
    {
        /// <inheritdoc/>
        private readonly IRoleInfoMapper _mapper;
        private readonly RoleRepository _repository;
        private readonly ILogger<FindRolesCommand> _logger;

        public FindRolesCommand(
            RoleRepository repository,
            IRoleInfoMapper mapper,
            ILogger<FindRolesCommand> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        /// <inheritdoc/>
        public RolesResponse Execute(int skipCount, int takeCount)
        {
            RolesResponse result = new();

            var dbRoles = _repository.Find(skipCount, takeCount, out int totalCount);

            result.TotalCount = totalCount;

            foreach (var dbRole in dbRoles)
            {
                result.Roles.Add(_mapper.Map(dbRole));
            }

            return result;
        }
    }
}
