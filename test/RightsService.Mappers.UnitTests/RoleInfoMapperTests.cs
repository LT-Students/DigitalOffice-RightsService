//using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
//using LT.DigitalOffice.RightsService.Models.Db;
//using LT.DigitalOffice.RightsService.Models.Dto.Models;
//using LT.DigitalOffice.UnitTestKernel;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;

//namespace LT.DigitalOffice.RightsService.Mappers.UnitTests.Models
//{
//  class RoleInfoMapperTests
//    {
//        public DbRoleLocalization _dbRole;
//        public List<UserInfo> _users;
//        public List<RightInfo> _rights;
//        public RoleInfo _expectedRoleInfo;

//        public IRoleInfoMapper _mapper;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _mapper = new RoleInfoMapper();

//            _dbRole = new DbRoleLocalization
//            {
//                Id = Guid.NewGuid(),
//                Name = "test name",
//                Description = "test description",
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy = Guid.NewGuid()
//            };

//            _users = new List<UserInfo>
//            {
//                new UserInfo
//                {
//                    Id = Guid.NewGuid()
//                }
//            };

//            _rights = new List<RightInfo>
//            {
//                new RightInfo
//                {
//                    Id = 123
//                }
//            };

//            _expectedRoleInfo = new RoleInfo
//            {
//                Id = _dbRole.Id,
//                Name = _dbRole.Name,
//                Description = _dbRole.Description,
//                CreatedAtUtc = _dbRole.CreatedAt,
//                CreatedBy = _dbRole.CreatedBy,
//                Users = _users,
//                Rights = _rights
//            };
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenDbRoleIsNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, null, null));
//        }

//        [Test]
//        public void ShouldReturnRoleInfoSuccessful()
//        {
//            SerializerAssert.AreEqual(_expectedRoleInfo, _mapper.Map(_dbRole, _rights, _users));
//        }
//    }
//}
