namespace ApiContracts;

public class UserDto
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required int Points { get; set; }
}