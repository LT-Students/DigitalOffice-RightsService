using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.RightsService.Mappers
{
    public class UserInfoMapper : IUserInfoMapper
    {
        public UserInfo Map(UserData userData)
        {
            if (userData == null)
            {
                throw new ArgumentNullException(nameof(userData));
            }

            return new UserInfo
            {
                Id = userData.Id,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                MiddleName = userData.MiddleName
            };
        }
    }
}