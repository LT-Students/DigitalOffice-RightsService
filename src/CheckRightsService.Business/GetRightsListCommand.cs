using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Database.Entities;
using LT.DigitalOffice.CheckRightsService.Mappers.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using LT.DigitalOffice.CheckRightsService.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.CheckRightsService.Commands
{
    public class GetRightsListCommand : IGetRightsListCommand
    {
        private readonly ICheckRightsRepository repository;
        private readonly IMapper<DbRight, Right> mapper;

        public GetRightsListCommand([FromServices] ICheckRightsRepository repository,
            [FromServices] IMapper<DbRight, Right> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public List<Right> Execute()
        {
            return repository.GetRightsList().Select(right => mapper.Map(right)).ToList();
        }
    }
}