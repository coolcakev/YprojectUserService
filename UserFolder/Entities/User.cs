namespace YprojectUserService.UserFolder.Entities;

public enum SexType
{
    Male,
    Female
}

public enum AgeGroup
{
    Teenagers,
    Young,
    ActiveDevelopment,
    AdultLife,
    LateMaturity,
    ActiveLongevity
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
    
    public AgeGroup DetermineAgeGroup()
    {
        var today = DateTime.Today;
        int age = today.Year - Birthday.Year;
        if (Birthday.Date > today.AddYears(-age))
        {
            age--;
        }

        return age switch
        {
            >= 14 and <= 17 => AgeGroup.Teenagers,
            >= 18 and <= 24 => AgeGroup.Young,
            >= 25 and <= 34 => AgeGroup.ActiveDevelopment,
            >= 35 and <= 44 => AgeGroup.AdultLife,
            >= 45 and <= 54 => AgeGroup.LateMaturity,
            _ => AgeGroup.ActiveLongevity
        };
    }
} 