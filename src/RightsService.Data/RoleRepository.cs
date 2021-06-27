using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDataProvider _provider;

        public RoleRepository(
            IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid Create(DbRole dbRole)
        {
            _provider.Roles.Add(dbRole);
            _provider.Save();

            return dbRole.Id;
        }

        public IEnumerable<DbRole> Find(int skipCount, int takeCount, out int totalCount)
        {
            totalCount = _provider.Users.Count();


            return _provider.Roles
                .Skip(skipCount * takeCount)
                .Take(takeCount)
                .Include(x => x.Rights)
                .Include(x => x.Users)
                .ToList();
        }
    }
}