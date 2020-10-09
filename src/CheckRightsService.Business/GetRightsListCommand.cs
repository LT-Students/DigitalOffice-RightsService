using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Mappers.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.CheckRightsService.Business
{
    /// <inheritdoc cref="IGetRightsListCommand"/>
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