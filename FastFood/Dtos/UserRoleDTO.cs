namespace FastFood.Dtos;

public class UserRoleDTO
{
    public int Id { get; set; }
    public int User_FK { get; set; }
    public int Role_FK { get; set; }

    public UserDTO User { get; set; }
    public RoleDTO Role { get; set; }
}
