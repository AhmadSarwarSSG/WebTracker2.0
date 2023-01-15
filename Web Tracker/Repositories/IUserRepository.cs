using System.Collections.Generic;
using System.Threading.Tasks;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        Task<List<User>> GetUsersByWebsiteId(int websiteId);
        User GetUserById(int id);
        bool AddUser(User user);
        bool DeleteUser(int id);
        bool UpdateUser(int id, User user);
        List<Address> GetAddresses();
    }
}