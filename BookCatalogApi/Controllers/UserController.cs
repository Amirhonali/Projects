using Application.DTOs.UserDTO;
using Application.Extensions;
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
    public class UserController : ApiControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _permissionRepository;

        public UserController(IRoleRepository permissionRepository, IUserRepository userRepository)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }
        [CustomAuthorizationFilter("GetUserById")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User Id:{id} not found!");
            }
            return Ok(user);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<User> Users = _userRepository.Get(x => true);

            return Ok(Users);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDto)
        {
            User user = _mapper.Map<User>(userCreateDto);
            List<Role> permissions = new();
            for (int i = 0; i < user.Roles.Count; i++)
            {
                Role permission = user.Roles.ToArray()[i];

                permission = await _permissionRepository.GetByIdAsync(permission.RoleId);
                if (permission == null)
                {
                    return NotFound($"Role not found");
                }
                permissions.Add(permission);
            }
            user.Roles = permissions;
            user = await _userRepository.AddAsync(user);
            if (user == null) return BadRequest(ModelState);

            UserGetDTO userGet = _mapper.Map<UserGetDTO>(user);
            return Ok(userGet);


        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDto)
        {
            User user = _mapper.Map<User>(userUpdateDto);

            user = await _userRepository.UpdateAsync(user);
            if (user == null) return BadRequest(ModelState);

            UserGetDTO userGet = _mapper.Map<UserGetDTO>(user);
            return Ok(userGet);


        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            bool isDelete = await _userRepository.DeleteAsync(id);
            return isDelete ? Ok("Deleted successfully")
                : BadRequest("Delete operation failed");
        }
        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeUserPassword(UserChangePasswordDTO userChangePassword)
        {
                var user = await _userRepository.GetByIdAsync(userChangePassword.UserId);

                if (user != null)
                {
                    string CurrentHash = userChangePassword.CurrentPassword.GetHash();
                    if (CurrentHash == user.Password
                        && userChangePassword.NewPassword == userChangePassword.ConfirmNewPassword)
                    {
                        user.Password = userChangePassword.NewPassword.GetHash();
                        await _userRepository.UpdateAsync(user);
                        return Ok();
                    }
                    else return BadRequest("Incorrect password");
                }
                return BadRequest("User not found");
            
        }
    }
}
