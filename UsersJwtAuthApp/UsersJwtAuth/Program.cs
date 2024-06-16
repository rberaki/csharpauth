using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersJwtAuth.Models;
using UsersJwtAuth.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/users", async (IUserRepository repo) =>
{
    var result = await repo.GetAll();
    return Results.Ok(result);
});

app.MapPost("users/register", async (IUserRepository repo, [FromBody] UserDto userDto) =>
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
});

app.MapPost("/users/login", async (IUserRepository repo, IConfiguration configuration, [FromBody] UserLoginDto userDto) =>
{
    var user = await repo.GetByUsername(userDto.Username);

    // TODO: login and get access token
    
    return Results.Ok(new TokenDto("", ""));
});

app.MapPut("/users/{id}", async (IUserRepository repo, int id, [FromBody] UserDto userDto) =>
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
});

app.MapDelete("/users/{id}", async (IUserRepository repo, int id) =>
{
    await repo.Delete(id);

    return Results.Ok();
});

app.Run();
