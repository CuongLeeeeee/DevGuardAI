using DevGuardAI.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevGuardAI.DAL.Interfaces
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
