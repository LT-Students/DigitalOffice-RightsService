using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using System;

namespace LT.DigitalOffice.RightsService.Mappers.Interfaces
{
    [AutoInject]
    public interface IDbRoleMapper
    {
        DbRole Map(CreateRoleRequest value);
    }
}