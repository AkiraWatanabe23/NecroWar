using System.Collections.Generic;

public enum SearchDirection
{
    None,
    Upper,
    UpperRight,
    Right,
    LowerRight,
    Lower,
    LowerLeft,
    Left,
    UpperLeft,
}

public class PieceMovementSearch
{
    private readonly int[,] _board = default;

    private readonly Dictionary<SearchDirection, int[]> _searchDirection = new()
    {
        { SearchDirection.None,       new int[] { 0, 0 } },
        { SearchDirection.Upper,      new int[] { 0, 1 } },
        { SearchDirection.UpperRight, new int[] { 1, 1 } },
        { SearchDirection.Right,      new int[] { 1, 0 } },
        { SearchDirection.LowerRight, new int[] { 1, -1 } },
        { SearchDirection.Lower,      new int[] { 0, -1 } },
        { SearchDirection.LowerLeft,  new int[] { -1, -1 } },
        { SearchDirection.Left,       new int[] { -1, 0 } },
        { SearchDirection.UpperLeft,  new int[] { -1, 1 } },
    };

    private const int BoardSize = 8;

    public int this[int row, int column]
    {
        get => _board[row, column];
        set => _board[row, column] = value;
    }

    /// <summary> 指定したマスが空いているかの判定を行う </summary>
    public bool TryGetCell(int row, int column)
        => 0 <= row && row < BoardSize && 0 <= column && column < BoardSize && _board[row, column] == (int)PieceType.None;


    /// <summary> 各方向の進行マスを調べる基底関数 </summary>
    private int GetMovableLength(int currentRow, int currentColumn, SearchDirection direction)
    {
        //指定された方向に何マス進めるか表現する値
        int count = 0;
        int rowDirection = _searchDirection[direction][0];
        int columnDireciton = _searchDirection[direction][1];

        for (int row = currentRow + rowDirection, column = currentColumn + columnDireciton;
            TryGetCell(row, column);
            row += rowDirection, column += columnDireciton)
        {
            count++;
        }

        return count;
    }

    /// <summary> 指定方向に何マス進めるかを調べる（左） </summary>
    public int TryMoveLeft(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.Left);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（左上） </summary>
    public int TryMoveUpperLeft(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.UpperLeft);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（上） </summary>
    public int TryMoveUp(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.Upper);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（右上） </summary>
    public int TryMoveUpperRight(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.UpperRight);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（右） </summary>
    public int TryMoveRight(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.Right);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（右下） </summary>
    public int TryMoveLowerRight(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.LowerRight);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（下） </summary>
    public int TryMoveLow(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.Lower);
    }

    /// <summary> 指定方向に何マス進めるかを調べる（左下） </summary>
    public int TryMoveLowerLeft(int row, int column)
    {
        return GetMovableLength(row, column, SearchDirection.LowerLeft);
    }
}
