using Application.DTOs.PermissionDTO;
using Application.Repositories;
using Application.UseCases.Permission.Commands;
using Application.UseCases.Permission.Query;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogApi.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class PermissionController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPermissionById([FromQuery] GetPermissionByIdQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPermissions()
        {
            return await _mediator.Send(new GetAllPermissionQuery());
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionCommand permissionCreateDto)
        {
            return await _mediator.Send(permissionCreateDto);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePermission([FromBody] CreatePermissionCommand PermissionUpdateDto)
        {
            return await _mediator.Send(PermissionUpdateDto);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeletePermission([FromQuery] DeletePermissionCommand command)
        {
            return await _mediator.Send(command);
        }

    }
}
