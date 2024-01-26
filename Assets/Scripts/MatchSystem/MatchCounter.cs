using UnityEngine;

public enum MatchDirection
{
    Horizontal, Vertical, Two_Sided
}
public class MatchCounter : MonoBehaviour
{
    public static MatchCounter Instance;
    [SerializeField] private CandyProvider candyProvider;
    private const int minimumMatchToGetNewCandy = 4;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion

    }
    public void CountMatches(MatchesDTO matches)
    {
        bool anyGeneration = ControlHorizontalMatchGeneration(matches);

        if (!anyGeneration)
        {
            anyGeneration = ControlVerticalMatchGeneration(matches);
        }
        if (!anyGeneration)
        {
            anyGeneration = ControlWrappedCandyMatch(matches);
        }
    }
    public bool ControlHorizontalMatchGeneration(MatchesDTO matches)
    {
        int counter = 1;
        foreach (Candy candy in matches.GetHorizontalMatches())
        {
            if (candy != null && candy.isMatched && candy.Type == matches.GetSwipedCandy().Type)
            {
                counter++;
            }
        }

        if (counter >= minimumMatchToGetNewCandy)
        {
            GameObject result = candyProvider.GetResultCandy(counter, matches.GetSwipedCandy().Type, MatchDirection.Horizontal);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return result != null ? true : false;
        }

        return false;
    }
    public bool ControlVerticalMatchGeneration(MatchesDTO matches)
    {
        int counter = 1;
        foreach (Candy candy in matches.GetVerticalMatches())
        {
            if (candy != null && candy.isMatched && candy.Type == matches.GetSwipedCandy().Type)
            {
                counter++;
            }
        }

        if (counter >= minimumMatchToGetNewCandy)
        {
            GameObject result = candyProvider.GetResultCandy(counter, matches.GetSwipedCandy().Type, MatchDirection.Vertical);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return result != null ? true : false;
        }

        return false;
    }
    public bool ControlWrappedCandyMatch(MatchesDTO matches)
    {
        #region MatchAlgorithm
        if (AreCandiesNotNull(matches.left1, matches.right1, matches.up1, matches.up2) &&
            AreCandiesAreMatched(matches.left1, matches.right1, matches.up1, matches.up2) &&
            AreCandiesSameType(matches.GetSwipedCandy(), matches.left1, matches.right1, matches.up1, matches.up2))
        {
            GameObject result = candyProvider.GetResultCandy(6, matches.GetSwipedCandy().Type, MatchDirection.Two_Sided);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return true;
        }

        else if (AreCandiesNotNull(matches.left1, matches.right1, matches.down1, matches.down2) &&
            AreCandiesAreMatched(matches.left1, matches.right1, matches.down1, matches.down2) &&
            AreCandiesSameType(matches.GetSwipedCandy(), matches.left1, matches.right1, matches.down1, matches.down2)
           )
        {
            GameObject result = candyProvider.GetResultCandy(6, matches.GetSwipedCandy().Type, MatchDirection.Two_Sided);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return true;
        }

        else if (AreCandiesNotNull(matches.right1, matches.right2, matches.down1, matches.up1) &&
            AreCandiesAreMatched(matches.right1, matches.right2, matches.down1, matches.up1) &&
            AreCandiesSameType(matches.GetSwipedCandy(), matches.right1, matches.right2, matches.down1, matches.up1)
            )
        {
            GameObject result = candyProvider.GetResultCandy(6, matches.GetSwipedCandy().Type, MatchDirection.Two_Sided);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return true;
        }
        else if (
            AreCandiesNotNull(matches.left1, matches.left2, matches.down1, matches.up1) &&
            AreCandiesAreMatched(matches.left1, matches.left2, matches.down1, matches.up1) &&
            AreCandiesSameType(matches.GetSwipedCandy(), matches.left1, matches.left2, matches.down1, matches.up1)
            )
        {
            GameObject result = candyProvider.GetResultCandy(6, matches.GetSwipedCandy().Type, MatchDirection.Two_Sided);
            ControlResultCandy(result, matches.GetSwipedCandy());
            return true;
        }
        #endregion
        return false;
    }
    public void ControlResultCandy(GameObject result, Candy swipedCandy)
    {
        if (result != null)
        {
            Board.Instance.AddMultiMatchCandy(result, swipedCandy.Column, swipedCandy.Row);
        }
    }
    private bool AreCandiesNotNull(params Candy[] candies)
    {
        foreach (Candy candy in candies)
        {
            if (candy == null) return false;
        }
        return true;
    }
    private bool AreCandiesAreMatched(params Candy[] candies)
    {
        foreach (Candy candy in candies)
        {
            if (!candy.isMatched) return false;
        }
        return true;
    }
    private bool AreCandiesSameType(Candy source, params Candy[] candies)
    {

        foreach (Candy candy in candies)
        {
            if (candy.Type != source.Type) return false;
        }
        return true;
    }
}
