using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.RoleDTO;

public class RoleCreateDTO
{
    public string Name { get; set; }

    public int[] PermissionsId { get; set; }

}
