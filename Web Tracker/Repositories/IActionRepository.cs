using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;
namespace WebTracker.Repositories
{
    public interface IActionRepository
    {
        List<Models.Action> GetAllActions();
        Models.Action GetActionById(int id);
        bool AddAction(Models.Action action);
        bool DeleteAction(int id);
        bool UpdateAction(int id, Models.Action action);
    }
}