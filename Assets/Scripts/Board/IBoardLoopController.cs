
using System.Collections.Generic;

public interface IBoardLoopController
{
    public void HandleMatches();
    public void DecreaseRows();
    public void RefillBoard();
    public void CreateMultimatchCandies();
    public bool AnyNewMatch();
}
