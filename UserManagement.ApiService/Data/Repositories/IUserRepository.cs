using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<(int, bool)> LogInUser(string email, string password);
    }
}
