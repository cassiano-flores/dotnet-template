using DotnetTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace DotnetTemplate.Repositories;

public sealed class RefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetTokenByToken(string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<RefreshToken> CreateToken(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);

        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task UpdateToken(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveToken(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllTokensByUserId(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            token.RevokedAt = now;
        }

        await _context.SaveChangesAsync();
    }
}
