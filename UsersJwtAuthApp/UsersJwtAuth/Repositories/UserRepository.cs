using Microsoft.EntityFrameworkCore;
using UsersJwtAuth.Models;

namespace UsersJwtAuth.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAll()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> GetById(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User> GetByUsername(string username)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task Add(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
