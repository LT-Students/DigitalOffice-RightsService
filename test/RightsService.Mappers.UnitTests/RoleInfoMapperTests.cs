using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Mappers.UnitTests.Models
{
    class RoleInfoMapperTests
    {
        public DbRole _dbRole;
        public List<UserInfo> _users;
        public List<RightResponse> _rights;
        public RoleInfo _expectedRoleInfo;

        public IRoleInfoMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new RoleInfoMapper();

            _dbRole = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = "test name",
                Description = "test description",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid()
            };

            _users = new List<UserInfo>
            {
                new UserInfo
                {
                    Id = Guid.NewGuid()
                }
            };

            _rights = new List<RightResponse>
            {
                new RightResponse
                {
                    Id = 123
                }
            };

            _expectedRoleInfo = new RoleInfo
            {
                Id = _dbRole.Id,
                Name = _dbRole.Name,
                Description = _dbRole.Description,
                CreatedAt = _dbRole.CreatedAt,
                CreatedBy = _dbRole.CreatedBy,
                Users = _users,
                Rights = _rights
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenDbRoleIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, null, null));
        }

        [Test]
        public void ShouldReturnRoleInfoSuccessful()
        {
            SerializerAssert.AreEqual(_expectedRoleInfo, _mapper.Map(_dbRole, _rights, _users));
        }
    }
}
