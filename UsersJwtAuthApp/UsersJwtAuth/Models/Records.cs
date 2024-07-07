namespace UsersJwtAuth.Models;

public record UserLoginDto(string Username, string Password);

public record UserDto (string Firstname, string Lastname, string Email, string Username, string Password);
