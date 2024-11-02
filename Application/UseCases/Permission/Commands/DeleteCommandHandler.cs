using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.UseCases.Permission.Commands
{
    public class DeletePermissionCommand : IRequest<IActionResult>
    {
        public int Id { get; set; }
    }
    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, IActionResult>
    {
        private readonly IPermissionRepository _permissionRepository;

        public DeletePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IActionResult> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            bool isDelete = await _permissionRepository.DeleteAsync(request.Id);

            return isDelete ? new OkObjectResult("Deleted successfully")
                : new BadRequestObjectResult("Delete operation failed");
        }
    }
}
