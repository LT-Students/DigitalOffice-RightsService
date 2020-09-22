using LT.DigitalOffice.CheckRightsService.Database;
using LT.DigitalOffice.CheckRightsService.Database.Entities;
using LT.DigitalOffice.CheckRightsService.Mappers.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using Moq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.CheckRightsService.Data.Provider;

namespace LT.DigitalOffice.CheckRightsServiceUnitTests.Repositories
{
    public class CheckRightsRepositoryTests
    {
        private IDataProvider provider;
        private ICheckRightsRepository repository;
        private Mock<IMapper<DbRight, Right>> mapperMock;
        private DbRight dbRight1InDb;
        private DbRight dbRight2InDb;
        private Guid userId;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<CheckRightsServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            provider = new CheckRightsServiceDbContext(dbOptions);
            mapperMock = new Mock<IMapper<DbRight, Right>>();
            repository = new CheckRightsRepository(provider);

            userId = Guid.NewGuid();
            dbRight1InDb = new DbRight
            {
                Id = 3,
                Name = "Right",
                Description = "Allows you everything",
                RightUsers = new List<DbRightUser>()
            };
            dbRight1InDb.RightUsers.Add(
                new DbRightUser
                {
                    RightId = dbRight1InDb.Id,
                    UserId = userId
                });

            dbRight2InDb = new DbRight
            {
                Id = 4,
                Name = "Right update",
                Description = "Allows you update everything",
                RightUsers = new List<DbRightUser>()
            };
            dbRight1InDb.RightUsers.Add(
                new DbRightUser
                {
                    RightId = dbRight2InDb.Id,
                    UserId = userId
                });

            provider.Rights.AddRange(dbRight1InDb, dbRight2InDb);
            provider.SaveChanges();

            mapperMock.Setup(mapper => mapper.Map(dbRight1InDb)).Returns(new Right
                {Id = dbRight1InDb.Id, Name = dbRight1InDb.Name, Description = dbRight1InDb.Description});

        }

        [TearDown]
        public void Clear()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        #region GetRightsList
        [Test]
        public void ShouldGetRightsListWhenDbIsNotEmpty()
        {
            Assert.That(repository.GetRightsList(), Is.EquivalentTo(provider.Rights.ToList()));
        }

        [Test]
        public void ShouldGetRightListWhenDbIsEmpty()
        {
            provider.Rights.RemoveRange(provider.Rights);
            provider.SaveChanges();

            Assert.That(repository.GetRightsList(), Is.Not.Null);
            Assert.That(provider.Rights, Is.Empty);
        }
        #endregion

        #region AddRightsForUser
        [Test]
        public void ShouldAddRightsForUser()
        {
            var rightId = dbRight1InDb.Id;
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int> { rightId }
            };

            var rightsBeforeRequest = provider.Rights.ToList();
            var rightUsersBeforeRequest = provider.RightUsers.ToList();
            Assert.IsNotNull(rightsBeforeRequest.FirstOrDefault(
                x => x.Id == rightId));
            Assert.IsNull(rightUsersBeforeRequest.FirstOrDefault(
                x => x.UserId == request.UserId && x.RightId == rightId));

            repository.AddRightsToUser(request);

            var rightsAfterRequest = provider.Rights.ToList();
            var rightUsersAfterRequest = provider.RightUsers.ToList();
            foreach(var right in rightsBeforeRequest)
            {
                Assert.IsTrue(rightsAfterRequest.Contains(right));
            }
            Assert.IsNotNull(rightUsersAfterRequest.FirstOrDefault(
                x => x.UserId == request.UserId && x.RightId == rightId));
            Assert.AreEqual(rightUsersBeforeRequest.Count + 1, rightUsersAfterRequest.Count);
        }

        [Test]
        public void ShouldThrowExceptionWhenRightIdIsNoFound()
        {
            var request = new AddRightsForUserRequest
            {
                UserId = Guid.NewGuid(),
                RightsIds = new List<int> { int.MaxValue, 0 }
            };

            Assert.Throws<BadRequestException>(() => repository.AddRightsToUser(request));
        }
        #endregion

        #region RemoveRightsFromUser
        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            var request = new RemoveRightsFromUserRequest
            {
                UserId = userId,
                RightIds = new List<int> { dbRight1InDb.Id }
            };

            var rightsBeforeRequest = provider.Rights.ToList();
            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = provider.Rights.ToList();
            var userRightsAfterRequest = provider.RightUsers.ToList();

            // Rights have not been removed.
            Assert.AreEqual(rightsBeforeRequest, rightsAfterRequest);
            // Removed required rights.
            userRightsBeforeRequest.RemoveAll(ru =>
                ru.UserId == request.UserId && request.RightIds.Contains(ru.RightId));
            Assert.AreEqual(userRightsBeforeRequest, userRightsAfterRequest);

        }

        [Test]
        public void ShouldNotDeleteAnythingWhenRightIdIsNotFound()
        {
            var request = new RemoveRightsFromUserRequest
            {
                UserId = userId,
                RightIds = new List<int> { int.MaxValue, 0 }
            };

            var rightsBeforeRequest = provider.Rights.ToList();
            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = provider.Rights.ToList();
            var userRightsAfterRequest = provider.RightUsers.ToList();

            // Rights have not been removed.
            Assert.AreEqual(rightsBeforeRequest, rightsAfterRequest);
            // User rights have not been removed.
            Assert.AreEqual(userRightsBeforeRequest, userRightsAfterRequest);
        }

        [Test]
        public void ShouldNotDeleteAnythingWhenUserIdIsNoFound()
        {
            var request = new RemoveRightsFromUserRequest
            {
                UserId = Guid.NewGuid(),
                RightIds = new List<int> { dbRight1InDb.Id }
            };

            var rightsBeforeRequest = provider.Rights.ToList();
            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = provider.Rights.ToList();
            var userRightsAfterRequest = provider.RightUsers.ToList();

            // Rights have not been removed.
            Assert.AreEqual(rightsBeforeRequest, rightsAfterRequest);
            // User rights have not been removed.
            Assert.AreEqual(userRightsBeforeRequest, userRightsAfterRequest);
        }
        #endregion
    }
}