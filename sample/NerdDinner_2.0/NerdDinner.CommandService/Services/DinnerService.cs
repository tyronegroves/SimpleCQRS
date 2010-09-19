using System;
using System.Linq;
using NerdDinner.CommandService.Models;

namespace NerdDinner.CommandService.Services
{
    public interface IDinnerService
    {
        bool DinnerExists(Guid dinnerId);
    }

    public class DinnerService : IDinnerService
    {
        private readonly NerdDinnerEntities db = new NerdDinnerEntities();

        public bool DinnerExists(Guid dinnerId)
        {
            return db.DinnerReadModels.Any(dinner => dinner.DinnerId == dinnerId);
        }
    }
}