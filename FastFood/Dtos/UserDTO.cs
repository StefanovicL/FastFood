using FastFood.Models;

namespace FastFood.Dtos;

public class UserDTO
{
    public UserDTO()
    {
        UserRoles = new HashSet<UserRoleDTO>();
    }

    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }

    public virtual ICollection<UserRoleDTO> UserRoles { get; set; }
}
