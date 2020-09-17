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

namespace LT.DigitalOffice.CheckRightsServiceUnitTests.Repositories
{
    public class CheckRightsRepositoryTests
    {
        private CheckRightsServiceDbContext dbContext;
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
            dbContext = new CheckRightsServiceDbContext(dbOptions);
            mapperMock = new Mock<IMapper<DbRight, Right>>();
            repository = new CheckRightsRepository(dbContext);

            userId = Guid.NewGuid();
            dbRight1InDb = new DbRight
            {
                Id = 3,
                Name = "Right",
                Description = "Allows you everything"
            };
            dbContext.RightUsers.Add(new DbRightUser
            {
                RightId = dbRight1InDb.Id,
                UserId = userId
            });

            dbRight2InDb = new DbRight
            {
                Id = 4,
                Name = "Right update",
                Description = "Allows you update everything",
            };
            dbContext.RightUsers.Add(new DbRightUser
            {
                Right = dbRight2InDb,
                RightId = dbRight2InDb.Id,
                UserId = userId
            });

            dbContext.Rights.AddRange(dbRight1InDb, dbRight2InDb);
            dbContext.SaveChanges();

            mapperMock.Setup(mapper => mapper.Map(dbRight1InDb)).Returns(new Right
                {Id = dbRight1InDb.Id, Name = dbRight1InDb.Name, Description = dbRight1InDb.Description});

        }

        [TearDown]
        public void Clear()
        {
            if (dbContext.Database.IsInMemory())
            {
                dbContext.Database.EnsureDeleted();
            }
        }

        #region GetRightsList
        [Test]
        public void ShouldGetRightsListWhenDbIsNotEmpty()
        {
            Assert.That(repository.GetRightsList(), Is.EquivalentTo(dbContext.Rights.ToList()));
        }

        [Test]
        public void ShouldGetRightListWhenDbIsEmpty()
        {
            dbContext.Rights.RemoveRange(dbContext.Rights);
            dbContext.SaveChanges();

            Assert.That(repository.GetRightsList(), Is.Not.Null);
            Assert.That(dbContext.Rights, Is.Empty);
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

            var rightsBeforeRequest = dbContext.Rights.ToList();
            var rightUsersBeforeRequest = dbContext.RightUsers.ToList();
            Assert.IsNotNull(rightsBeforeRequest.FirstOrDefault(
                x => x.Id == rightId));
            Assert.IsNull(rightUsersBeforeRequest.FirstOrDefault(
                x => x.UserId == request.UserId && x.RightId == rightId));

            repository.AddRightsToUser(request);

            var rightsAfterRequest = dbContext.Rights.ToList();
            var rightUsersAfterRequest = dbContext.RightUsers.ToList();
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

            var rightsBeforeRequest = dbContext.Rights.ToList();
            var userRightsBeforeRequest = dbContext.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = dbContext.Rights.ToList();
            var userRightsAfterRequest = dbContext.RightUsers.ToList();

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

            var rightsBeforeRequest = dbContext.Rights.ToList();
            var userRightsBeforeRequest = dbContext.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = dbContext.Rights.ToList();
            var userRightsAfterRequest = dbContext.RightUsers.ToList();

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

            var rightsBeforeRequest = dbContext.Rights.ToList();
            var userRightsBeforeRequest = dbContext.RightUsers.ToList();

            repository.RemoveRightsFromUser(request);

            var rightsAfterRequest = dbContext.Rights.ToList();
            var userRightsAfterRequest = dbContext.RightUsers.ToList();

            // Rights have not been removed.
            Assert.AreEqual(rightsBeforeRequest, rightsAfterRequest);
            // User rights have not been removed.
            Assert.AreEqual(userRightsBeforeRequest, userRightsAfterRequest);
        }
        #endregion
    }
}