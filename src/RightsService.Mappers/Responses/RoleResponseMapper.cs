using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;

namespace LT.DigitalOffice.RightsService.Mappers.Responses
{
  public class RoleResponseMapper : IRoleResponseMapper
  {
    private readonly IRoleInfoMapper _roleInfoMapper;
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IRightInfoMapper _rightMapper;

    public RoleResponseMapper(
      IRoleInfoMapper roleInfoMapper,
      IUserInfoMapper userInfoMapper,
      IRightInfoMapper rightMapper)
    {
      _roleInfoMapper = roleInfoMapper;
      _userInfoMapper = userInfoMapper;
      _rightMapper = rightMapper;
    }

    public RoleResponse Map(DbRole role, List<DbRightsLocalization> rights, List<UserData> users)
    {
      if (role == null)
      {
        return null;
      }

      var userInfos = users?.Select(_userInfoMapper.Map).ToList();

      return new RoleResponse
      {
        Role = _roleInfoMapper.Map(role, rights.Select(_rightMapper.Map).ToList(), userInfos),
        Users = userInfos?.Where(ui => role.Users.Any(ud => ud.UserId == ui.Id)).ToList()
      };
    }
  }
}
