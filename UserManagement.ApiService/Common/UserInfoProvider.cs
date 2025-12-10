using UserManagement.ApiService.Common.Views;

namespace UserManagement.ApiService.Common;

public class UserInfoProvider
{
    public UserInfo UserInfo { get; set; }

    public UserInfoProvider()
    {
        UserInfo = new UserInfo(); // Ensure it's initialized by default
    }
}