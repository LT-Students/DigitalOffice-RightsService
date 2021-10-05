//using LT.DigitalOffice.Kernel.Broker;
//using LT.DigitalOffice.Kernel.Exceptions.Models;
//using LT.DigitalOffice.Models.Broker.Models;
//using LT.DigitalOffice.Models.Broker.Requests.User;
//using LT.DigitalOffice.Models.Broker.Responses.User;
//using LT.DigitalOffice.RightsService.Business.Role;
//using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
//using LT.DigitalOffice.RightsService.Data.Interfaces;
//using LT.DigitalOffice.RightsService.Mappers.Interfaces;
//using LT.DigitalOffice.RightsService.Models.Db;
//using LT.DigitalOffice.RightsService.Models.Dto.Models;
//using LT.DigitalOffice.RightsService.Models.Dto.Responses;
//using LT.DigitalOffice.UnitTestKernel;
//using MassTransit;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Moq.AutoMock;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace LT.DigitalOffice.RightsService.Business.Commands.Role
//{
//    class GetRoleCommandTests
//    {
//        private AutoMocker _mocker;

//        private Guid _validRoleId;

//        private DbUser _dbUser;
//        private DbRight _dbRight;
//        private DbRole _dbRole;

//        private UserData _userData;
//        private UserInfo _userInfo;
//        private RoleInfo _roleInfo;

//        private RightResponse _rightResponse;
//        private RoleResponse _roleResponse;

//        private IGetRoleCommand _command;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _mocker = new AutoMocker();

//            _validRoleId = Guid.NewGuid();

//            _dbUser = new DbUser() { UserId = Guid.NewGuid() };
//            _dbRight = new DbRight() { Id = 1, Name = "rightName", Description = "rightDescription" };
//            _dbRole = new DbRole()
//            {
//                Id = _validRoleId,
//                Name = "roleName",
//                Description = "roleDescr",
//                CreatedBy = Guid.NewGuid(),
//                CreatedAt = DateTime.UtcNow,
//                IsActive = true,
//                Rights = new List<DbRoleRight>() { new DbRoleRight() { Right = _dbRight } },
//                Users = new List<DbUser>() { _dbUser }
//            };

//            _userData = new UserData(_dbUser.UserId, Guid.NewGuid(), "fName", "mName", "lName", "s", 1.0f, true);
//            _userInfo = new UserInfo()
//            {
//                Id = _userData.Id,
//                FirstName = _userData.FirstName,
//                LastName = _userData.LastName,
//                MiddleName = _userData.MiddleName
//            };
//            _roleInfo = new RoleInfo
//            {
//                Id = _dbRole.Id,
//                Name = _dbRole.Name,
//                Description = _dbRole.Description,
//                CreatedAt = _dbRole.CreatedAt,
//                CreatedBy = _dbRole.CreatedBy,
//                Rights = new List<RightResponse>() { _rightResponse },
//                Users = new List<UserInfo>() { _userInfo }
//            };

//            _rightResponse = new RightResponse()
//            {
//                Id = _dbRight.Id,
//                Name = _dbRight.Name,
//                Description = _dbRight.Description
//            };

//            _roleResponse = new RoleResponse()
//            {
//                Role = _roleInfo,
//                Errors = new List<string>()
//            };

//            _command = _mocker.CreateInstance<GetRoleCommand>();
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            _mocker.GetMock<Response<IOperationResult<IGetUsersDataResponse>>>().Reset();
//            _mocker.GetMock<IRoleRepository>().Reset();
//            _mocker.GetMock<IRightRepository>().Reset();
//            _mocker.GetMock<IRoleInfoMapper>().Reset();
//            _mocker.GetMock<IUserInfoMapper>().Reset();
//            _mocker.GetMock<IRightResponseMapper>().Reset();
//            _mocker.GetMock<IRequestClient<IGetUsersDataRequest>>().Reset();
//        }

//        [Test]
//        public void ShouldThrowNotFoundExceptionWhedRoleIdIsInvalid()
//        {
//            _mocker
//                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
//                .Throws(new NotFoundException());

//            Assert.Throws<NotFoundException>(() => _command.Execute(Guid.NewGuid()));

//            _mocker.Verify<IRoleRepository>(r => r.Get(It.IsAny<Guid>()), Times.Once());
//            _mocker.Verify<IRightResponseMapper>(m => m.Map(It.IsAny<DbRight>()), Times.Never());
//            _mocker.Verify<IRequestClient<IGetUsersDataRequest>>(r =>
//                r.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Never());
//            _mocker.Verify<IUserInfoMapper>(m => m.Map(It.IsAny<UserData>()), Times.Never());
//            _mocker.Verify<IRoleInfoMapper>(m =>
//                m.Map(It.IsAny<DbRole>(), It.IsAny<List<RightResponse>>(), It.IsAny<List<UserInfo>>()), Times.Never());
//        }

//        [Test]
//        public void ShouldGetAnErrorsWhenConsumerIsNotResponseCorrectly()
//        {
//            List<string> responedErrors = new List<string>() { "err1", "err2" };
//            int expectedErrorsCount = responedErrors.Count;

//            _mocker
//                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
//                .Returns(_dbRole);

//            _mocker
//                .Setup<IRightResponseMapper, RightResponse>(m => m.Map(It.IsNotNull<DbRight>()))
//                .Returns(_rightResponse);

//            _mocker.
//                Setup<Response<IOperationResult<IGetUsersDataResponse>>, bool>(r => r.Message.IsSuccess)
//                .Returns(false);

//            _mocker
//                .Setup<Response<IOperationResult<IGetUsersDataResponse>>, List<string>>(r => r.Message.Errors)
//                .Returns(responedErrors);

//            _mocker
//                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>
//                (
//                    x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>
//                    (
//                        It.IsAny<object>(), default, default
//                    )
//                )
//                .Returns(Task.FromResult(_mocker.GetMock<Response<IOperationResult<IGetUsersDataResponse>>>().Object));

//            RoleResponse result = _command.Execute(_dbRole.Id);

//            Assert.AreEqual(result.Errors.Count, expectedErrorsCount);

//            _mocker.Verify<IRoleRepository>(r => r.Get(It.IsAny<Guid>()), Times.Once());
//            _mocker.Verify<IRightResponseMapper>(m => m.Map(It.IsAny<DbRight>()), Times.Once());
//            _mocker.Verify<IRequestClient<IGetUsersDataRequest>>(r =>
//                r.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once());
//            _mocker.Verify<IUserInfoMapper>(m => m.Map(It.IsAny<UserData>()), Times.Never());
//            _mocker.Verify<IRoleInfoMapper>(m => 
//                m.Map(_dbRole, It.Is<List<RightResponse>>(lr => lr[0] == _rightResponse), It.Is<List<UserInfo>>(lu => lu.Count == 0)), 
//                Times.Once());
//        }

//        [Test]
//        public void ShouldReturnCorrectResult()
//        {
//            _mocker
//                .Setup<IRoleRepository, DbRole>(r => r.Get(It.IsAny<Guid>()))
//                .Returns(_dbRole);

//            _mocker
//                .Setup<IRightResponseMapper, RightResponse>(m => m.Map(It.IsNotNull<DbRight>()))
//                .Returns(_rightResponse);

//            _mocker
//                .Setup<Response<IOperationResult<IGetUsersDataResponse>>, bool>(r => r.Message.IsSuccess)
//                .Returns(true);

//            _mocker
//                .Setup<Response<IOperationResult<IGetUsersDataResponse>>, List<string>>(r => r.Message.Errors)
//                .Returns(new List<string>());

//            _mocker
//               .Setup<Response<IOperationResult<IGetUsersDataResponse>>, List<UserData>>(r => r.Message.Body.UsersData)
//               .Returns(new List<UserData>() { _userData });

//            _mocker
//                .Setup<IUserInfoMapper, UserInfo>(m => m.Map(_userData))
//                .Returns(_userInfo);

//            _mocker
//                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>
//                (
//                    x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>
//                    (
//                        It.IsAny<object>(), default, default
//                    )
//                )
//                .Returns(Task.FromResult(_mocker.GetMock<Response<IOperationResult<IGetUsersDataResponse>>>().Object));

//            _mocker
//                .Setup<IRoleInfoMapper, RoleInfo>(m =>
//                    m.Map(_dbRole, new List<RightResponse>() { _rightResponse }, new List<UserInfo>() { _userInfo }))
//                .Returns(_roleInfo);

//            SerializerAssert.AreEqual(_roleResponse, _command.Execute(_dbRole.Id));

//            _mocker.Verify<IRoleRepository>(r => r.Get(It.IsAny<Guid>()), Times.Once());
//            _mocker.Verify<IRightResponseMapper>(m => m.Map(It.IsAny<DbRight>()), Times.Once());
//            _mocker.Verify<IRequestClient<IGetUsersDataRequest>>(r =>
//                r.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once());
//            _mocker.Verify<IUserInfoMapper>(m => m.Map(It.IsAny<UserData>()), Times.Once());
//            _mocker.Verify<IRoleInfoMapper>(m =>
//                m.Map(_dbRole, It.Is<List<RightResponse>>(lr => lr[0] == _rightResponse), It.Is<List<UserInfo>>(lu => lu[0] == _userInfo)),
//                Times.Once());
//        }
//    }
//}
