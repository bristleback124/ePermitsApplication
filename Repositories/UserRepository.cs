using ePermits.Models;
using Microsoft.EntityFrameworkCore;
using ePermitsApp.Data;
using System.Threading.Tasks;

namespace ePermits.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.UserProfile)
                .Include(u => u.LGU)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.UserProfile)
                .Include(u => u.LGU)
                .Include(u => u.Department)
                .ToListAsync();
        }

        public async Task<List<User>> GetAssignableReviewersAsync(int departmentId)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.UserProfile)
                .Include(u => u.Department)
                .Where(u =>
                    u.UserRole != null &&
                    (u.UserRole.UserRoleDesc == "admin" || u.UserRole.UserRoleDesc == "superadmin" || u.UserRole.UserRoleDesc == "sysadmin"
                    || (u.UserRole.UserRoleDesc == "user" && u.DepartmentId == departmentId)))
                .OrderBy(u => u.UserProfile != null ? u.UserProfile.FirstName : u.Username)
                .ThenBy(u => u.UserProfile != null ? u.UserProfile.LastName : u.Username)
                .ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.UserProfile)
                .Include(u => u.LGU)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var userProfile = await _context.UserProfiles
                .Include(up => up.User)
                    .ThenInclude(u => u!.UserRole)
                .Include(up => up.User)
                    .ThenInclude(u => u!.LGU)
                .Include(up => up.User)
                   .ThenInclude(u => u!.Department)
                .FirstOrDefaultAsync(up => up.Email == email);

            return userProfile?.User;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.UserProfiles
                .Where(up => up.UserId != 0) // Only check profiles with valid user references
                .AnyAsync(up => up.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
