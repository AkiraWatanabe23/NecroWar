using System.Collections.Generic;

public class SamplePiece : IPiece
{
    public List<int[]> MovableArea => new();

    public void Initialize(PieceMovementSearch movementBase)
    {

    }

    public List<int[]> SearchMovableArea(int row, int column)
    {
        MovableArea.Clear();

        return MovableArea;
    }
}
