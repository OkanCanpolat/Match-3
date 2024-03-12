
public class BasicCandyMatchFinder : IMatchFinder
{
    private bool horizontalmatch;
    private bool verticalmatch;
    private MatchesDTO matches;
    private Board board;
    private MatchCounter matchCounter;

    public BasicCandyMatchFinder(Board board, MatchCounter matchCounter)
    {
        this.board = board;
        this.matchCounter = matchCounter;
    }
    public bool FindMatch(Candy source, MatchesDTO resultMatches, bool markMatches = true)
    {
        matches = resultMatches;
        horizontalmatch = FindHorizontalMatch(source, markMatches);
        verticalmatch = FindVerticalMatch(source, markMatches);
        ControlMatchCounter(markMatches);
       
        return horizontalmatch | verticalmatch;
    }
    private bool FindHorizontalMatch(Candy source, bool markIsMatched)
    {
        bool matched = false;

        Candy right1 = board.TryGetDrop(source.CandyProperties.Column + 1, source.CandyProperties.Row);
        Candy right2 = board.TryGetDrop(source.CandyProperties.Column + 2, source.CandyProperties.Row);
        Candy left1 = board.TryGetDrop(source.CandyProperties.Column - 1, source.CandyProperties.Row);
        Candy left2 = board.TryGetDrop(source.CandyProperties.Column - 2, source.CandyProperties.Row);

        matched = HandleTripleMatch(source, right1, left1, markIsMatched) | matched;
        matched = HandleTripleMatch(source, right1, right2, markIsMatched) | matched;
        matched = HandleTripleMatch(source, left1, left2, markIsMatched) | matched;

        matches.InitHorizontalMatches(right1, right2, left1, left2);

        return matched;
    }
    private bool FindVerticalMatch(Candy source, bool markIsMatched)
    {
        bool matched = false;

        Candy up1 = board.TryGetDrop(source.CandyProperties.Column, source.CandyProperties.Row + 1);
        Candy up2 = board.TryGetDrop(source.CandyProperties.Column, source.CandyProperties.Row + 2);
        Candy down1 = board.TryGetDrop(source.CandyProperties.Column, source.CandyProperties.Row - 1);
        Candy down2 = board.TryGetDrop(source.CandyProperties.Column, source.CandyProperties.Row - 2);

        matched = HandleTripleMatch(source, up1, down1, markIsMatched) | matched;
        matched = HandleTripleMatch(source, down1, down2, markIsMatched) | matched;
        matched = HandleTripleMatch(source, up1, up2, markIsMatched) | matched;

        matches.InitVerticalMatches(up1, up2, down1, down2);

        return matched;
    }
    private bool HandleTripleMatch(Candy source, Candy first, Candy second, bool markIsMatched)
    {
        bool matched = false;

        if (first != null && second != null && first.Type == source.Type && second.Type == source.Type)
        {
            if (markIsMatched)
            {
                first.isMatched = true;
                second.isMatched = true;
                source.isMatched = true;
            }
            matched = true;
        }

        return matched;
    }
    private void ControlMatchCounter(bool markMatches)
    {
        if ((horizontalmatch || verticalmatch) && markMatches)
        {
            matchCounter.CountMatches(matches);
        }
    }
}
