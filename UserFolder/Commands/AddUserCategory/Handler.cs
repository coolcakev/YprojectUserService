using MediatR;
using Microsoft.AspNetCore.Mvc;
using y_nuget.Endpoints;
using YprojectUserService.Database;
using YprojectUserService.Localization;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.UserFolder.Commands.AddUserCategory;

public record AddUserCategoryRequest(string Id, [FromBody] AddUserCategoryBody Body) : IHttpRequest<EmptyValue>;

public record CategoryIds(
    Guid CategoryId, 
    Guid CategoryTitleId
);
public record AddUserCategoryBody(
    CategoryIds MainCategory,
    List<CategoryIds> SubCategories
);

public class Handler : IRequestHandler<AddUserCategoryRequest, Response<EmptyValue>>
{
    private readonly ApplicationDbContext _dbContext;
    
    public Handler(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    // якщо вже має категорії ??
    public async Task<Response<EmptyValue>> Handle(AddUserCategoryRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(request.Id);

        if (user is null)
        {
            return FailureResponses.NotFound(LocalizationKeys.User.NotFound);
        }
            
        var mainCategory = new UserCategory
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CategoryRole = CategoryType.Primary,
            CategoryId = request.Body.MainCategory.CategoryId,
            CategoryTitleId = request.Body.MainCategory.CategoryTitleId
        };
        
        _dbContext.UserCategories.Add(mainCategory);
        
        foreach (var subCategory in request.Body.SubCategories)
        {
            var userCategory = new UserCategory
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CategoryRole = CategoryType.Addition,
                CategoryId = subCategory.CategoryId,
                CategoryTitleId = subCategory.CategoryTitleId
            };

            _dbContext.UserCategories.Add(userCategory);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return SuccessResponses.Ok();
    }
}