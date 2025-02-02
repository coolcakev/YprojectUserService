namespace YprojectUserService.UserFolder.Entities;

//TODO притримуватися одного стиля в енамках
public enum SexType
{
    Male,
    Female
}

public enum AgeGroup
{
    Teenagers = 0,
    Young = 1,
    ActiveDevelopment = 2,
    AdultLife = 3,
    LateMaturity = 4,
    ActiveLongevity = 5
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
    public AgeGroup AgeGroup { get; set; }
    public SexType Sex { get; set; }
    public string CountryISO { get; set; }
    public string StateISO { get; set; }
    public int CityId { get; set; }
    public ICollection<UserCategory> Categories { get; set; } = new List<UserCategory>();
} 