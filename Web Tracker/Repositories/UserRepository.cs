using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SignalRCheck;
using WebTracker.Models;


namespace WebTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WebTrackerDBContext _context;
        public UserRepository(WebTrackerDBContext context) => _context = context;
        public List<User> GetAllUsers() => _context.Users.ToList();
        public User GetUserById(int id) => _context.Users.FirstOrDefault(u => u.UserId == id);
        public bool AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteUser(int id)
        {
            var user = _context.Users
                        .Where(x => x.UserId == id)
                        .FirstOrDefault();
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdateUser(int id, User user)
        {
            var userToUpdate = _context
                                .Users
                                .Where(u => u.UserId == id)
                                .FirstOrDefault();
            if(userToUpdate != null)
            {
                userToUpdate.DeviceType = user.DeviceType;
                userToUpdate.Browser = user.Browser;
                userToUpdate.LastConnection = user.LastConnection;
                userToUpdate.Address = user.Address;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetUsersByWebsiteId(int websiteId)
        {
            return await _context.Users
            .Where(u => u.WebsiteId == websiteId)
            .ToListAsync();
        }
        public List<Address> GetAddresses()
        {
            return _context.Addresses.ToList();
        }
    }
}