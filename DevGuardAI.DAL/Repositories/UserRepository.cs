using DevGuardAI.DAL.Data;
using DevGuardAI.DAL.Entities;
using DevGuardAI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevGuardAI.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DevGuardAIDbContext context) : base(context)
        {
        }
    }
}
