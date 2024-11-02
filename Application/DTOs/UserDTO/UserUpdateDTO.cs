using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTO;

public class UserUpdateDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public int[] RolesId{ get; set; }
}
