using MassTransit;
using y_nuget.RabbitMq;
using YprojectUserService.Database;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.RabbitMq;

public class UserCategoryConsumerService : IConsumer<AddUserCategoryRequestDto>
{
    private readonly ApplicationDbContext _dbContext;

    public UserCategoryConsumerService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<AddUserCategoryRequestDto> context)
    {
        var messageWrapper = context.Message;
        
        Console.WriteLine("[X] Message:");
        Console.WriteLine(messageWrapper);
        
        var user = await _dbContext.Users.FindAsync(messageWrapper.UserId);
 
        if (user is null)
        {
            Console.WriteLine($"[*] Error: User '{messageWrapper.UserId}' not found!");
            throw new Exception($"User '{messageWrapper.UserId}' not found!");
        }

        var userCategory = new List<UserCategory>();
        
        foreach (var category in messageWrapper.Categories)
        {
            userCategory.Add(new UserCategory
            {
                Id = Guid.NewGuid(),
                UserId = messageWrapper.UserId,
                CategoryId = category.CategoryId,
                CategoryRole = category.CategoryType,
                CategoryTitleKey = category.CategoryTitleKey
            });
        }
        
        await _dbContext.UserCategories.AddRangeAsync(userCategory);
        await _dbContext.SaveChangesAsync();
    }
}