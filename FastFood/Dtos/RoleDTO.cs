namespace FastFood.Dtos;

public class RoleDTO
{
    public RoleDTO()
    {
        UserRoles = new HashSet<UserRoleDTO>();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<UserRoleDTO> UserRoles { get; set; }
}
