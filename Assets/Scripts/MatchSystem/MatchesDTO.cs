using System;
using System.Collections.Generic;

[Serializable]
public class MatchesDTO
{
    #region PossibleMatchCandies
    public Candy right1;
    public Candy right2;
    public Candy left1;
    public Candy left2;
    public Candy up1;
    public Candy up2;
    public Candy down1;
    public Candy down2;
    #endregion

    private Candy swipedCandy;
    private List<Candy> horizontalMatches;
    private List<Candy> verticalMatches;

    public MatchesDTO(Candy swipedCandy)
    {
        this.swipedCandy = swipedCandy;
        horizontalMatches = new List<Candy>();
        verticalMatches = new List<Candy>();
    }
    public void InitHorizontalMatches(Candy right1, Candy right2, Candy left1, Candy left2)
    {
        horizontalMatches.Clear();
        this.right1 = right1;
        this.right2 = right2;
        this.left1 = left1;
        this.left2 = left2;
        horizontalMatches.Add(right1);
        horizontalMatches.Add(right2);
        horizontalMatches.Add(left1);
        horizontalMatches.Add(left2);

    }
    public void InitVerticalMatches(Candy up1, Candy up2, Candy down1, Candy down2)
    {
        verticalMatches.Clear();
        this.up1 = up1;
        this.up2 = up2;
        this.down1 = down1;
        this.down2 = down2;
        verticalMatches.Add(up1);
        verticalMatches.Add(up2);
        verticalMatches.Add(down1);
        verticalMatches.Add(down2);

    }
    public Candy GetSwipedCandy()
    {
        return swipedCandy;
    }
    public List<Candy> GetHorizontalMatches()
    {
        return horizontalMatches;
    }
    public List<Candy> GetVerticalMatches()
    {
        return verticalMatches;
    }
}
