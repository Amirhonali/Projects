using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Permission.Query
{
    public class GetAllPermissionQuery:IRequest<IActionResult>
    {

    }
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionQuery, IActionResult>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IActionResult> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
        {
            var Permissions = _permissionRepository.Get(x => true).ToList();

            return new OkObjectResult(Permissions);
        }
    }
}
