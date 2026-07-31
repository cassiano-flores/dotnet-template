using Common.Models;
using DotnetTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace DotnetTemplate.Repositories;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLowerInvariant());
    }

    public async Task<User?> GetUserByPasswordResetToken(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.PasswordResetToken == token);
    }

    public async Task<User> CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUser(User user)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task RemoveUser(User user)
    {
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }
}
