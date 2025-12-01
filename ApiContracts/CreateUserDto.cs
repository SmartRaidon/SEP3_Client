namespace ApiContracts;

public class CreateUserDto
{
    public required string Username { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;  
}