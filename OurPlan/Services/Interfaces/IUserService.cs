using OurPlan.Entity;

namespace OurPlan.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Register(string username, string email, string password);
        string Login(string email, string password);
    }
}
