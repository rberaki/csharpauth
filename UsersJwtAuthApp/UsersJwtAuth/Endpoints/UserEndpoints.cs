using Microsoft.AspNetCore.Mvc;
using UsersJwtAuth.Models;
using UsersJwtAuth.Repositories;

namespace UsersJwtAuth.Endpoints;

public static class UserEndpoints
{
    public static void AddUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users", GetUsers);

        app.MapPost("users/register", CreateUser);

        app.MapPost("/users/login", GetToken);

        app.MapPut("/users/{id}", UpdateUser);

        app.MapDelete("/users/{id}", DeleteUser);
    }

    private async static Task<IResult> GetUsers(IUserRepository repo)
    {
        var result = await repo.GetAll();
        return Results.Ok(result);
    }

    private async static Task<IResult> CreateUser(IUserRepository repo, [FromBody] UserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            Password = userDto.Password, // TODO: Hash password
            Email = userDto.Email,
            FirstName = userDto.Firstname,
            LastName = userDto.Lastname
        };

        await repo.Add(user);

        return Results.Ok();
    }

    private async static Task<IResult> GetToken(IUserRepository repo, IConfiguration configuration, [FromBody] UserLoginDto userDto)
    {
        var user = await repo.GetByUsername(userDto.Username);

        // TODO: login and get access token

        return Results.Ok(new TokenDto("", ""));
    }

    private async static Task<IResult> UpdateUser(IUserRepository repo, int id, [FromBody] UserDto userDto)
    {
        var user = await repo.GetById(id);

        if (user == null)
            return Results.NotFound("User not found");

        user.FirstName = userDto.Firstname;
        user.LastName = userDto.Lastname;
        user.Email = userDto.Email;
        user.Password = userDto.Password; // TODO: Hash password

        await repo.Update(user);

        return Results.Ok();
    }

    private async static Task<IResult> DeleteUser(IUserRepository repo, int id)
    {
        await repo.Delete(id);

        return Results.Ok();
    }
}
