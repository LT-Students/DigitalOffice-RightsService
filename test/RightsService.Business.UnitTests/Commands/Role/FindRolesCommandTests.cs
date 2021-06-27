using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
    class FindRolesCommandTests
    {
        private IEnumerable<DbRole> _dbRoles;
        private IEnumerable<RoleInfo> _rolesInfo;

        private AutoMocker _mocker;
        private IFindRolesCommand _command;
        private IDictionary<object, object> _contextValues;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO add fill
        }

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<IFindRolesCommand>();
        }

        // TODO add tests
    }
}
