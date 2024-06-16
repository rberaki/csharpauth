namespace UsersJwtAuth.Models;

public record UserLoginDto(string Username, string Password);

public record UserDto (int Id, string Firstname, string Lastname, string Email, string Username, string Password);

public record TokenDto (string AccessToken, string TokenType);
