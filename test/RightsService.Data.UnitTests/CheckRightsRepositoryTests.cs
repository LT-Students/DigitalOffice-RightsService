using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Exceptions.Models;

namespace LT.DigitalOffice.RightsService.Data.UnitTests
{
    public class CheckRightsRepositoryTests
    {
        public class OperationResult<T> : IOperationResult<T>
        {
            public bool IsSuccess { get; set; }

            public List<string> Errors { get; set; }

            public T Body { get; set; }
        }

        private IDataProvider provider;
        private ICheckRightsRepository repository;
        private Mock<IRightsMapper> mapperMock;
        private Mock<IRequestClient<IGetUserRequest>> clientMock;
        private OperationResult<IGetUserResponse> operationResult;
        private DbRight dbRight1InDb;
        private DbRight dbRight2InDb;
        private Guid userId;
        private IEnumerable<int> rightsIds;

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<RightsServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            provider = new RightsServiceDbContext(dbOptions);
            mapperMock = new Mock<IRightsMapper>();
            clientMock = new Mock<IRequestClient<IGetUserRequest>>();
            BrokerSetUp();

            repository = new CheckRightsRepository(provider, clientMock.Object);

            userId = Guid.NewGuid();
            dbRight1InDb = new DbRight
            {
                Id = 3,
                Name = "Right",
                Description = "Allows you everything",
                Users = new List<DbUserRight>()
            };
            provider.RightUsers.Add(
                new DbUserRight
                {
                    RightId = dbRight1InDb.Id,
                    UserId = userId
                });

            dbRight2InDb = new DbRight
            {
                Id = 4,
                Name = "Right update",
                Description = "Allows you update everything",
                Users = new List<DbUserRight>()
            };
            provider.RightUsers.Add(
                new DbUserRight
                {
                    RightId = dbRight2InDb.Id,
                    UserId = userId
                });

            provider.Rights.AddRange(dbRight1InDb, dbRight2InDb);
            provider.Save();

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

        private void BrokerSetUp()
        {
            var responseClientMock = new Mock<Response<IOperationResult<IGetUserResponse>>>();
            clientMock = new Mock<IRequestClient<IGetUserRequest>>();

            operationResult = new OperationResult<IGetUserResponse>();

            clientMock.Setup(
                x => x.GetResponse<IOperationResult<IGetUserResponse>>(
                    It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseClientMock.Object));

            responseClientMock
                .SetupGet(x => x.Message)
                .Returns(operationResult);
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
            provider.Save();

            Assert.That(repository.GetRightsList(), Is.Not.Null);
            Assert.That(provider.Rights, Is.Empty);
        }
        #endregion

        #region AddRightsForUser
        [Test]
        public void ShouldAddRightsForUser()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();

            var rightId = dbRight1InDb.Id;
            userId = Guid.NewGuid();
            rightsIds = new List<int> { rightId };

            var rightsBeforeRequest = provider.Rights.ToList();
            var rightUsersBeforeRequest = provider.RightUsers.ToList();
            Assert.IsNotNull(rightsBeforeRequest.FirstOrDefault(
                x => x.Id == rightId));
            Assert.IsNull(rightUsersBeforeRequest.FirstOrDefault(
                x => x.UserId == userId && x.RightId == rightId));

            repository.AddRightsToUser(userId, rightsIds);

            var rightsAfterRequest = provider.Rights.ToList();
            var rightUsersAfterRequest = provider.RightUsers.ToList();
            foreach(var right in rightsBeforeRequest)
            {
                Assert.IsTrue(rightsAfterRequest.Contains(right));
            }
            Assert.IsNotNull(rightUsersAfterRequest.FirstOrDefault(
                x => x.UserId == userId && x.RightId == rightId));
            Assert.AreEqual(rightUsersBeforeRequest.Count + 1, rightUsersAfterRequest.Count);
        }

        [Test]
        public void ShouldThrowExceptionWhenRightIdIsNoFound()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();

            userId = Guid.NewGuid();
            rightsIds = new List<int> { int.MaxValue, 0 };

            Assert.Throws<BadRequestException>(() => repository.AddRightsToUser(userId, rightsIds));
        }

        [Test]
        public void ShouldThrowExceptionWhenUserIdIsNoFound()
        {
            operationResult.IsSuccess = false;
            operationResult.Errors = new List<string>();

            userId = Guid.NewGuid();
            rightsIds = new List<int> { dbRight1InDb.Id };

            Assert.Throws<NotFoundException>(() => repository.AddRightsToUser(userId, rightsIds));
        }
        #endregion

        #region RemoveRightsFromUser
        [Test]
        public void ShouldRemoveRightsFromUser()
        {
            rightsIds = new List<int> { dbRight1InDb.Id };

            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(userId, rightsIds);

            var rightsAfterRequest = provider.Rights.ToList();
            var userRightsAfterRequest = provider.RightUsers.ToList();


            var rightsBeforeRequest = provider.Rights.ToList();
            // Rights have not been removed.
            Assert.AreEqual(rightsBeforeRequest, rightsAfterRequest);
            // Removed required rights.
            userRightsBeforeRequest.RemoveAll(ru =>
                ru.UserId == userId && rightsIds.Contains(ru.RightId));
            Assert.AreEqual(userRightsBeforeRequest, userRightsAfterRequest);

        }

        [Test]
        public void ShouldNotDeleteAnythingWhenRightIdIsNotFound()
        {
            rightsIds = new List<int> { int.MaxValue, 0 };

            var rightsBeforeRequest = provider.Rights.ToList();
            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(userId, rightsIds);

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
            userId = Guid.NewGuid();
            rightsIds = new List<int> { dbRight1InDb.Id };

            var rightsBeforeRequest = provider.Rights.ToList();
            var userRightsBeforeRequest = provider.RightUsers.ToList();

            repository.RemoveRightsFromUser(userId, rightsIds);

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