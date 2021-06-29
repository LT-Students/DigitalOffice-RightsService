using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Data.UnitTests
{
    public class RoleRepositoryTests
    {
        private IDataProvider _provider;
        private IRoleRepository _repository;

        private DbRole _dbRole;
        private DbRight _dbRight;

        private DbContextOptions<RightsServiceDbContext> _dbContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var roleId = Guid.NewGuid();

            _dbRole = new DbRole
            {
                Id = roleId,
                Name = "test name",
                Description = "test description",
                CreatedAt = DateTime.Now,
                CreatedBy = Guid.NewGuid()
            };

            _dbContext = new DbContextOptionsBuilder<RightsServiceDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;

            // TODO fill data
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new RightsServiceDbContext(_dbContext);
            _repository = new RoleRepository(_provider);

            _provider.Roles.Add(_dbRole);
            _provider.Save();
        }

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        #region Get

        [Test]
        public void ShouldGetRoleWhenRoleExists()
        {
            Assert.AreEqual(_repository.Get(_dbRole.Id), _dbRole);
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhenRoleNotExists()
        {
            Assert.Throws<NotFoundException>(() => _repository.Get(Guid.NewGuid()));
        }

        #endregion

        #region Find

        [Test]
        public void ShouldGetRolesWhenDbIsNotEmpty()
        {
            int total;

            Assert.That(_repository.Find(0, 100, out total), Is.EquivalentTo(_provider.Roles.ToList()));
            Assert.AreEqual(total, _provider.Roles.ToList().Count);
        }

        #endregion

        #region Create

        [Test]
        public void ShouldCreateRole()
        {
            // TODO
        }

        #endregion
    }
}
