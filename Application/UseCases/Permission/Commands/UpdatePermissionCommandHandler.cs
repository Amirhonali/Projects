using Application.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.UseCases.Permission.Commands
{
    public class UpdatePermissionCommand : IRequest<IActionResult>
    {
        public int PermissionId { get; set; }

        [Column("permission_name")]
        public string? PermissionName { get; set; }
    }
    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, IActionResult>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public UpdatePermissionCommandHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Permission permission = _mapper.Map<Domain.Entities.Permission>(request);

            permission = await _permissionRepository.UpdateAsync(permission);
            if (permission == null) return new NotFoundResult();

            return new OkObjectResult(permission);
        }
    }
}
