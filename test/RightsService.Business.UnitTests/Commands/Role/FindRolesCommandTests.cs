using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using Moq.AutoMock;
using NUnit.Framework;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
    class FindRolesCommandTests
    {
        private IEnumerable<DbRoleLocalization> _dbRoles;
        private IEnumerable<UserInfo> _rolesInfo;

        private AutoMocker _mocker;
        private IFindRolesCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
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
