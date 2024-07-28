using UsersJwtAuth.Endpoints;
using UsersJwtAuth.StartupDependency;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddInfrastructureServices(builder.Configuration)
    .AddAuthServices(builder.Configuration)
    .AddSwaggerServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.AddUserEndpoints();

app.Run();
