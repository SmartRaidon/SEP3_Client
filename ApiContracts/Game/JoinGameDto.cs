namespace ApiContracts;

public class JoinGameDto
{
    public required string InviteCode { get; set; }
    public required int PlayerId { get; set; }
}