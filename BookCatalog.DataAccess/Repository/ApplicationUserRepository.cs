using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public override IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter = null, string includeProperties = "")
        {
            IEnumerable<ApplicationUser> users = base.GetAll(filter, includeProperties);
            foreach (ApplicationUser user in users) 
            {
                user.Role = _db.Roles.FirstOrDefault(r => r.Id == _db.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id).RoleId).Name;
            }
            return users;
        }
        public override ApplicationUser? Get(Expression<Func<ApplicationUser, bool>> filter, bool isTracked = false)
        {
            ApplicationUser? user = base.Get(filter, isTracked);
            user!.Role = _db!.Roles!.FirstOrDefault(r => r!.Id == _db.UserRoles!.FirstOrDefault(ur => ur!.UserId == user!.Id).RoleId).Name;
            return user;
        }

		public void LockUnlock(string id)
		{
            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(au => au.Id == id);
            if (user.LockoutEnd is not null && user.LockoutEnd > DateTime.Now) 
            {
                user.LockoutEnd = DateTime.Now;       
            }
            else 
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

		}

		public string GetRole(string id)
		{
			ApplicationUser user = Get(au => au.Id == id);
            return user!.Role;
		}
	}
}
