using Application.DTOs.RoleDTO;
using Application.Repositories;
using AutoMapper;
using BookCatalogApi.Filters;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ApiControllerBase
    {
        
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RoleController(IRoleRepository roleRepository,IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetRoleById([FromQuery] int id)
        {
            Role role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound($"Role Id:{id} not found!");
            }
            return Ok(role);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllRoles()
        {
            IEnumerable<Role> Roles = _roleRepository.Get(x => true);

            return Ok(Roles);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO roleCreateDto)
        {
            Role role = _mapper.Map<Role>(roleCreateDto);
            List<Permission> permissions = new();
            for (int i = 0; i < role.Permissions.Count; i++)
            {
                Permission permission = role.Permissions.ToArray()[i];

                permission = await _permissionRepository.GetByIdAsync(permission.PermissionId);
                if (permission == null)
                {
                    return NotFound($"Permission not found");
                }
                permissions.Add(permission);
            }
            role.Permissions = permissions;
            role = await _roleRepository.AddAsync(role);
            if (role == null) return BadRequest(ModelState);

            RoleGetDTO roleGet = _mapper.Map<RoleGetDTO>(role);
            return Ok(roleGet);


        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDTO roleUpdateDto)
        {
            Role role = _mapper.Map<Role>(roleUpdateDto);

            role = await _roleRepository.UpdateAsync(role);
            if (role == null) return BadRequest(ModelState);

            RoleGetDTO roleGet = _mapper.Map<RoleGetDTO>(role);
            return Ok(roleGet);


        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteRole([FromQuery] int id)
        {
            bool isDelete = await _roleRepository.DeleteAsync(id);
            return isDelete ? Ok("Deleted successfully")
                : BadRequest("Delete operation failed");
        }
    }
}
