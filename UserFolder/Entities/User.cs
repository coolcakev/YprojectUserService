namespace YprojectUserService.UserFolder.Entities;

public enum SexType
{
    Male,
    Female
}

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string CodeWord { get; set; }
    public DateTime Birthday { get; set; }
    public SexType Sex { get; set; }
} 