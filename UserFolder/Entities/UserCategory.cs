using y_nuget.Sources;

namespace YprojectUserService.UserFolder.Entities;

public class UserCategory
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public string UserId { get; set; } 
    public Guid CategoryId { get; set; } 
    public Guid CategoryTitleKey { get; set; }
    public CategoryType CategoryRole { get; set; } 
}