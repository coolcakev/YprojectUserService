namespace YprojectUserService.UserFolder.Entities;

public enum CategoryType
{
    Primary,
    Addition
}

public class UserCategory
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public string UserId { get; set; } 
    public Guid CategoryId { get; set; } 
    public Guid CategoryTitleId { get; set; }
    public CategoryType CategoryRole { get; set; } 
}