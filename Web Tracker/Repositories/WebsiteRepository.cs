using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;
using Microsoft.EntityFrameworkCore;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public class WebsiteRepository : IWebsiteRepository
    {
        private readonly WebTrackerDBContext _context;
        public WebsiteRepository(WebTrackerDBContext context) => _context = context;
        public bool AddWebsite(Website website)
        {
            Console.WriteLine("Adding Website to Database: " + website.Web);
            _context.Websites.Add(website);
            _context.SaveChanges();
            Console.WriteLine("Website successfully added to the database");
            return true;
        }
        public bool DeleteWebsite(int id)
        {
            var website = _context.Websites
                        .Where(x => x.WebsiteId == id)
                        .FirstOrDefault();
            if (website != null)
            {
                _context.Websites.Remove(website);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<Website>> GetAllWebsites()
        {
            Console.WriteLine("Getting List of Websites with All Details!");
            List<Website> websites = await _context.Websites
            .ToListAsync();
            // foreach(var website in websites)
            // {
            //     website.Users = await _context.Users.Where(x => x.WebsiteId == website.WebsiteId).ToListAsync();
            //     foreach(var user in website.Users)
            //     {
            //         user.Flows = await _context.Flows.Where(x => x.UserId == user.UserId).ToListAsync();
            //         foreach(var flow in user.Flows)
            //         {
            //             flow.Urls = await _context.Urls.Where(x => x.FlowId == flow.FlowId).ToListAsync();
            //             foreach(var url in flow.Urls)
            //             {
            //                 url.Actions = await _context.Actions.Where(x => x.UrlId == url.UrlId).ToListAsync();
            //                 foreach(var action in url.Actions)
            //                 {
            //                     action.Data = await _context.ActionsData.Where(x => x.ActionId == action.ActionId).ToListAsync();
            //                 }
            //             }
            //         }
            //     }
            // }
            Console.WriteLine("Got All Websites!");
            return websites;
        } 

        public async Task<Website> GetWebsiteById(int id) 
        {
            Console.WriteLine("Getting Website with ID = " + id);
            var website = await _context.Websites
            .Where(x => x.WebsiteId == id)
            .FirstOrDefaultAsync();
            // if (website != null)
            // {
            //     website.Users = await _context.Users.Where(x => x.WebsiteId == website.WebsiteId).ToListAsync();
            //     foreach(var user in website.Users)
            //     {
            //         user.Flows = await _context.Flows.Where(x => x.UserId == user.UserId).ToListAsync();
            //         foreach(var flow in user.Flows)
            //         {
            //             flow.Urls = await _context.Urls.Where(x => x.FlowId == flow.FlowId).ToListAsync();
            //             foreach(var url in flow.Urls)
            //             {
            //                 url.Actions = await _context.Actions.Where(x => x.UrlId == url.UrlId).ToListAsync();
            //                 foreach(var action in url.Actions)
            //                 {
            //                     action.Data = await _context.ActionsData.Where(x => x.ActionId == action.ActionId).ToListAsync();
            //                 }
            //             }
            //         }
            //     }
            // }
            Console.WriteLine("Website with ID = " + id + " Returned " + website.Web + "Website");
            return website;
        } 
        
        public int GetWebsiteIdByName(string website)
        {
            Console.WriteLine("Getting Website of name = " + website);
            var web = _context.Websites.FirstOrDefault(w => w.Web == website);
            
            if(web != null)
            {
                Console.WriteLine("************* &&&&&&&  Website of name = " + website + " returned id = " + web.WebsiteId);
                return web.WebsiteId;
            }
            Console.WriteLine("No Website Found of name = " + website);
            return -1;
        }

        public bool UpdateWebsite(int id, Website website)
        {
            var websiteToUpdate = _context.Websites.FirstOrDefault(w => w.WebsiteId == id);
            if(websiteToUpdate != null)
            {
                websiteToUpdate.Web = website.Web;
                websiteToUpdate.VisitCount = website.VisitCount;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}