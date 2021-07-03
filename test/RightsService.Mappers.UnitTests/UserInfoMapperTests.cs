using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.RightsService.Mappers.UnitTests.Models
{
    class UserInfoMapperTests
    {
        public UserData _userData;
        public UserInfo _expectedUserInfo;

        public IUserInfoMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new UserInfoMapper();

            _userData = new UserData(
                id: Guid.NewGuid(),
                firstName: "test name",
                lastName: "test lastname",
                middleName: "test middlename",
                isActive: true);

            _expectedUserInfo = new UserInfo
            {
                Id = _userData.Id,
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                MiddleName = _userData.MiddleName
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenUserDataIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }

        [Test]
        public void ShouldReturnUserInfoSuccessful()
        {
            SerializerAssert.AreEqual(_expectedUserInfo, _mapper.Map(_userData));
        }
    }
}
