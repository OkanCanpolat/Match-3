using UnityEngine;

public class DeaulfSwipeController : ISwipeController
{
    private Board board;
    public DeaulfSwipeController(Board board)
    {
        this.board = board;
    }
    public void ControlSwipe(Candy source, Vector2 swipeDirection)
    {
        int targetColumn = source.CandyProperties.Column + (int)swipeDirection.x;
        int targetRow = source.CandyProperties.Row + (int)swipeDirection.y;

        if (!board.IsInsideBoard(targetColumn, targetRow) || !board.IsLegalMovementTile(targetColumn, targetRow)) return;

        board.State = BoardState.CanNotMove;
        GameObject target = board.Columns[targetColumn].rows[targetRow];
        Candy targetDrop = target.GetComponent<Candy>();
        SwapDrops(source, targetDrop);
        source.Swap();
    }

    private void SwapDrops(Candy source, Candy target)
    {
        board.Columns[source.CandyProperties.Column].rows[source.CandyProperties.Row] = target.gameObject;
        board.Columns[target.CandyProperties.Column].rows[target.CandyProperties.Row] = source.gameObject;

        int sourceColumn = source.CandyProperties.Column;
        int sourceRow = source.CandyProperties.Row;

        source.CandyProperties.PreviousColumn = sourceColumn;
        source.CandyProperties.PreviousRow = sourceRow;
        target.CandyProperties.PreviousColumn = target.CandyProperties.Column;
        target.CandyProperties.PreviousRow = target.CandyProperties.Row;

        source.CandyProperties.Column = target.CandyProperties.Column;
        source.CandyProperties.Row = target.CandyProperties.Row;
        target.CandyProperties.Column = sourceColumn;
        target.CandyProperties.Row = sourceRow;

        source.CandyProperties.OtherCandy = target.gameObject;
    }
}
