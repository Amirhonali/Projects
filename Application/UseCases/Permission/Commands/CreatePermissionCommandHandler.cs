using Application.DTOs.PermissionDTO;
using Application.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Permission.Commands
{
   public class CreatePermissionCommand:IRequest<IActionResult>
    {
        public required string PermissionName { get; set; }
    }
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, IActionResult>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public CreatePermissionCommandHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Permission permission = _mapper.Map<Domain.Entities.Permission>(request);
            permission = await _permissionRepository.AddAsync(permission);
            if (permission == null) 
                return new BadRequestObjectResult("Not found");

            return new OkObjectResult(permission);
        }
    }
}
