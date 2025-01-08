namespace YprojectUserService.UserFolder.Entities;

public enum SexType
{
    Male,
    Female
}

public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    
    public bool IsEmailVerified { get; set; }
    public string Password { get; set; }
    public string CodeWord { get; set; }
    
    public string? RecoveryCode { get; set; }
    public DateTime Birthday { get; set; }
    public SexType Sex { get; set; }
    
    // TODO NEED COUNTRY LOGIC
} 