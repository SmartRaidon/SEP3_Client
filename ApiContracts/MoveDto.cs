namespace ApiContracts;

public class MoveDto
{
    public required int GameId { get; set; }
    public required int CellIndex { get; set; }
    public required string PlayerType { get; set; } // maybe its PlayerId instead
}