using System.Collections.Generic;

public interface IPiece
{
    public List<int[]> MovableArea { get; }

    public void Initialize(PieceMovementSearch movementBase);

    public List<int[]> SearchMovableArea(int row, int column);
}
