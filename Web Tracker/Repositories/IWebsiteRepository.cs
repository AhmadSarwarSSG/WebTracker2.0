using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public interface IWebsiteRepository
    {
        Task<List<Website>> GetAllWebsites();
        Task<Website> GetWebsiteById(int id);
        Task<int> GetWebsiteIdByName(string website);
        bool AddWebsite(Website website);
        bool DeleteWebsite(int id);
        bool UpdateWebsite(int id, Website website);
    }
}