using Application.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.UseCases.Permission.Query;

public class GetPermissionByIdQuery : IRequest<IActionResult>
{
    public int Id { get; set; }
}
public class GetPermissionByIdQueryHandlers : IRequestHandler<GetPermissionByIdQuery, IActionResult>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionByIdQueryHandlers(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IActionResult> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.Permission permission = await _permissionRepository.GetByIdAsync(request.Id);
        if (permission == null)
        {
            return new NotFoundObjectResult($"Permission Id:{request.Id} not found!");
        }
        return new OkObjectResult(permission);
    }
}
