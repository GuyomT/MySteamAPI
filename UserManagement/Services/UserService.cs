
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
