using ePermits.Models;
using ePermitsApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ePermits.Data
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole?> GetByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<UserRole?> GetByDescriptionAsync(string description)
        {
            return await _context.UserRoles
                .FirstOrDefaultAsync(r => r.UserRoleDesc.ToLower() == description.ToLower());
        }

        public async Task<List<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole> CreateAsync(UserRole role)
        {
            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<UserRole> UpdateAsync(UserRole role)
        {
            _context.UserRoles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.UserRoles.FindAsync(id);
            if (role == null)
                return false;

            _context.UserRoles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}