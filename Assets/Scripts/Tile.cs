
public class Tile {
	public const int EMPTY = -1;

	public int Position;
	public bool IsRollAgain;
	public bool IsSideline;
	public int PlayerStone;

	public Tile(int position, bool isRollAgain, bool isSideline) {
		Position = position;
		IsRollAgain = isRollAgain;
		IsSideline = isSideline;
		PlayerStone = EMPTY;
	}
}

