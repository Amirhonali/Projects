using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.RoleDTO;

public class RoleGetDTO
{
    public int RoleId { get; set; }

    public string Name { get; set; }

    public int[] PermissionsId { get; set; }

}
