using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Business.Role;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
    class GetRoleCommandTests
    {
        private AutoMocker _mocker;
        private Mock<Response<IOperationResult<IGetUsersDataResponse>>> _operationResultGetUsersData;

        private Guid _validRoleId;

        private DbUser _dbUser;
        private DbRight _dbRight;
        private DbRole _dbRole;

        private UserData _userData;
        private UserInfo _userInfo;
        private RoleInfo _roleInfo;

        private RightResponse _rightResponse;
        private RoleResponse _roleResponse;

        private IGetRoleCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mocker = new AutoMocker();
            _operationResultGetUsersData = new Mock<Response<IOperationResult<IGetUsersDataResponse>>>();

            _validRoleId = Guid.NewGuid();

            _dbUser = new DbUser() { UserId = Guid.NewGuid() };
            _dbRight = new DbRight() { Id = 1, Name = "rightName", Description = "rightDescription" };
            _dbRole = new DbRole()
            {
                Id = _validRoleId,
                Name = "roleName",
                Description = "roleDescr",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Rights = new List<DbRoleRight>() { new DbRoleRight() { Right = _dbRight } },
                Users = new List<DbUser>() { _dbUser }
            };

            _userData = new UserData(_dbUser.UserId, Guid.NewGuid(), "fName", "mName", "lName", "s", 1.0f, true);
            _userInfo = new UserInfo()
            {
                Id = _userData.Id,
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                MiddleName = _userData.MiddleName
            };
            _roleInfo = new RoleInfo
            {
                Id = _dbRole.Id,
                Name = _dbRole.Name,
                Description = _dbRole.Description,
                CreatedAt = _dbRole.CreatedAt,
                CreatedBy = _dbRole.CreatedBy,
                Rights = new List<RightResponse>() { _rightResponse },
                Users = new List<UserInfo>() { _userInfo }
            };

            _rightResponse = new RightResponse()
            {
                Id = _dbRight.Id,
                Name = _dbRight.Name,
                Description = _dbRight.Description
            };

            _roleResponse = new RoleResponse()
            {
                Role = _roleInfo,
                Errors = new List<string>()
            };

            _command = new GetRoleCommand(
                _mocker.GetMock<ILogger<GetRoleCommand>>().Object,
                _mocker.GetMock<IRoleRepository>().Object,
                _mocker.GetMock<IRightRepository>().Object,
                _mocker.GetMock<IRoleInfoMapper>().Object,
                _mocker.GetMock<IUserInfoMapper>().Object,
                _mocker.GetMock<IRightResponseMapper>().Object,
                _mocker.GetMock<IRequestClient<IGetUsersDataRequest>>().Object
            );
        }

        [SetUp]
        public void SetUp()
        {
            _operationResultGetUsersData.Reset();
            _mocker.GetMock<ILogger<GetRoleCommand>>();
            _mocker.GetMock<IRoleRepository>();
            _mocker.GetMock<IRightRepository>();
            _mocker.GetMock<IRoleInfoMapper>();
            _mocker.GetMock<IUserInfoMapper>();
            _mocker.GetMock<IRightResponseMapper>();
            _mocker.GetMock<IRequestClient<IGetUsersDataRequest>>();
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhedRoleIdIsInvalid()
        {
            _mocker
                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
                .Throws(new NotFoundException());

            Assert.Throws<NotFoundException>(() => _command.Execute(Guid.NewGuid()));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRightIsNull()
        {
            DbRole roleWithNullRight = new DbRole()
            {
                Rights = new List<DbRoleRight>()
                {
                    new DbRoleRight() { Right = null }
                }
            };

            _mocker
                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
                .Returns(roleWithNullRight);

            _mocker
                .Setup<IRightResponseMapper, RightResponse>(rrm => rrm.Map(It.Is<DbRight>(value => value == null)))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(Guid.NewGuid()));
        }

        [Test]
        public void ShouldGetAnErrorWhenConsumerReturnWrongUserData()
        {
            Guid invalidUserId = Guid.NewGuid();
            int expectedErrorsCount = 1;

            _mocker
                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
                .Returns(_dbRole);

            _mocker
                .Setup<IRightResponseMapper, RightResponse>(m => m.Map(It.IsNotNull<DbRight>()))
                .Returns(_rightResponse);

            _operationResultGetUsersData.Setup(r => r.Message.IsSuccess).Returns(true);
            _operationResultGetUsersData.Setup(r => r.Message.Errors).Returns(new List<string>());
            _operationResultGetUsersData
                .Setup(r => r.Message.Body.UsersData)
                .Returns(new List<UserData>() 
                {
                    // consumer return wrong UserData with invalid Id
                    new UserData(invalidUserId, Guid.NewGuid(), "fName", "mName", "lName", "s", 1.0f, true)
                });

            _mocker
                .Setup<IUserInfoMapper, UserInfo>(m => m.Map(It.IsAny<UserData>()))
                .Returns(new UserInfo());

            _mocker
                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>
                (
                    x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>
                    (
                        It.IsAny<object>(), default, default
                    )
                )
                .Returns(Task.FromResult(_operationResultGetUsersData.Object));

            RoleResponse result = _command.Execute(_dbRole.Id);

            Assert.AreEqual(result.Errors.Count, expectedErrorsCount);
        }

        [Test]
        public void ShouldGetAnErrorsWhenConsumerIsNotResponseCorrectly()
        {
            List<string> responedErrors = new List<string>() { "err1", "err2" };
            int expectedErrorsCount = responedErrors.Count;

            _mocker
                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
                .Returns(_dbRole);

            _mocker
                .Setup<IRightResponseMapper, RightResponse>(m => m.Map(It.IsNotNull<DbRight>()))
                .Returns(_rightResponse);

            _operationResultGetUsersData.Setup(r => r.Message.IsSuccess).Returns(false);
            _operationResultGetUsersData.Setup(r => r.Message.Errors).Returns(responedErrors);

            _mocker
                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>
                (
                    x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>
                    (
                        It.IsAny<object>(), default, default
                    )
                )
                .Returns(Task.FromResult(_operationResultGetUsersData.Object));

            RoleResponse result = _command.Execute(_dbRole.Id);

            Assert.AreEqual(result.Errors.Count, expectedErrorsCount);
        }

        [Test]
        public void ShouldReturnCorrectResult()
        {
            _mocker
                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
                .Returns(_dbRole);

            _mocker
                .Setup<IRightResponseMapper, RightResponse>(m => m.Map(It.IsNotNull<DbRight>()))
                .Returns(_rightResponse);

            _operationResultGetUsersData.Setup(r => r.Message.IsSuccess).Returns(true);
            _operationResultGetUsersData.Setup(r => r.Message.Errors).Returns(new List<string>());
            _operationResultGetUsersData
                .Setup(r => r.Message.Body.UsersData)
                .Returns(new List<UserData>() { _userData });

            _mocker
                .Setup<IUserInfoMapper, UserInfo>(m => m.Map(_userData))
                .Returns(_userInfo);

            _mocker
                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>
                (
                    x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>
                    (
                        It.IsAny<object>(), default, default
                    )
                )
                .Returns(Task.FromResult(_operationResultGetUsersData.Object));

            _mocker
                .Setup<IRoleInfoMapper, RoleInfo>(m =>
                    m.Map(_dbRole, new List<RightResponse>() { _rightResponse }, new List<UserInfo>() { _userInfo }))
                .Returns(_roleInfo);

            SerializerAssert.AreEqual(_roleResponse, _command.Execute(_dbRole.Id));
        }
    }
}
