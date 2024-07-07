using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using UsersJwtAuth.Models;
using UsersJwtAuth.Repositories;

namespace UsersJwtAuth.Endpoints;

public static class UserEndpoints
{
    public static void AddUserEndpoints(this WebApplication app)
    {
        app.MapPost("/token", GetToken).WithTags("Token");

        app.MapGet("/users", GetUsers).WithTags("Users");

        app.MapPost("users/register", CreateUser).WithTags("Users");

        app.MapPut("/users/{id}", UpdateUser).WithTags("Users");

        app.MapDelete("/users/{id}", DeleteUser).WithTags("Users");
    }

    [AllowAnonymous]
    private static async Task<IResult> GetToken(IUserRepository repo, IConfiguration config, [FromBody] UserLoginDto userLogin)
    {
        var user = await Login(repo, userLogin);

        if (user == null)
            return Results.Unauthorized();

        var token = CreateAccessToken(user, config);

        return Results.Ok(token);
    }

    private static async Task<IResult> GetUsers(IUserRepository repo)
    {
        var result = await repo.GetAll();
        return Results.Ok(result);
    }

    [AllowAnonymous]
    private static async Task<IResult> CreateUser(IUserRepository repo, [FromBody] UserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Email = userDto.Email,
            FirstName = userDto.Firstname,
            LastName = userDto.Lastname
        };

        await repo.Add(user);

        return Results.Ok();
    }

    private static async Task<IResult> UpdateUser(IUserRepository repo, int id, [FromBody] UserDto userDto)
    {
        var user = await repo.GetById(id);

        if (user == null)
            return Results.NotFound("User not found");

        user.FirstName = userDto.Firstname;
        user.LastName = userDto.Lastname;
        user.Email = userDto.Email;
        user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        await repo.Update(user);

        return Results.Ok();
    }

    private static async Task<IResult> DeleteUser(IUserRepository repo, int id)
    {
        await repo.Delete(id);

        return Results.Ok();
    }

    private static async Task<User?> Login(IUserRepository repo, UserLoginDto userLogin)
    {
        var user = await repo.GetByUsername(userLogin.Username);

        if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            return user;

        return null;
    }

    private static string CreateAccessToken(User user, IConfiguration config)
    {
        var securityKey = Encoding.UTF8.GetBytes(config.GetValue<string>("Auth:SecretKey")!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        ];

        var jwtSecurityToken = new JwtSecurityToken(
            config.GetValue<string>("Auth:Issuer"),
            config.GetValue<string>("Auth:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(config.GetValue<int>("Auth:TokenExpireMinutes")),
            credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
