namespace FastFood.Models;

public class UserRole
{
    public int Id { get; set; }
    public int User_FK { get; set; }
    public int Role_FK { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
