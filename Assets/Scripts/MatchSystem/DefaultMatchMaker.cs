public class DefaultMatchMaker : IMatchMaker
{
    private Board board;

    public DefaultMatchMaker(Board board)
    {
        this.board = board;
    }
    public void MatchAdjacentCandies(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > board.Width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > board.Height - 1) continue;

                if (board.Columns[i].rows[j] != null)
                {
                    Candy candy = board.Columns[i].rows[j].GetComponent<Candy>();
                    candy.isMatched = true;
                    candy.OnMatched();
                }
            }
        }
    }

    public void MatchAllCandiesWithTypeOf(CandyType type)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (board.Columns[i].rows[j] != null)
                {
                    Candy candy = board.Columns[i].rows[j].GetComponent<Candy>();

                    if (candy.Type == type)
                    {
                        candy.isMatched = true;
                    }
                }
            }
        }
    }

    public void MatchColumnCandies(int column, int row)
    {
        Board.Piece piece = board.Columns[column];

        for (int i = 0; i < board.Height; i++)
        {
            if (i != row && piece.rows[i] != null)
            {
                Candy candy = piece.rows[i].GetComponent<Candy>();
                candy.isMatched = true;
                candy.OnMatched();
            }
        }
    }

    public void MatchRowCandies(int column, int row)
    {
        for (int i = 0; i < board.Width; i++)
        {
            if (i != column && board.Columns[i].rows[row] != null)
            {
                Candy candy = board.Columns[i].rows[row].GetComponent<Candy>();
                candy.isMatched = true;
                candy.OnMatched();
            }
        }
    }
}
