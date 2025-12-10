using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.ApiService.Common;
using UserManagement.ApiService.Common.Views;
namespace UserManagement.ApiService.Filters;
public class UserInfoFilter : IActionFilter
{
    private readonly UserInfoProvider _userInfoProvider;

    public UserInfoFilter(UserInfoProvider userInfoProvider)
    {
        _userInfoProvider = userInfoProvider;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity.IsAuthenticated)
        {
            var userId = Guid.TryParse(user.FindFirst("ID")?.Value, out var id) ? id : Guid.Empty;
            _userInfoProvider.UserInfo.ID = userId;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}