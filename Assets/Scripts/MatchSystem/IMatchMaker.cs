
public interface IMatchMaker 
{
    public void MatchColumnCandies(int column, int row);
    public void MatchRowCandies(int column, int row);
    public void MatchAdjacentCandies(int column, int row);
    public void MatchAllCandiesWithTypeOf(CandyType type);
}
