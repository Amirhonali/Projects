using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Password { get; set; }

    public virtual ICollection<Role>? Roles { get; set; }
}
