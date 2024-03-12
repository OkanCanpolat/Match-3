using UnityEngine;

public class SwipeHandlerTangent : ISwipeHandler
{
    private CandySwipeSettings swipeSettings;
    private Board board;

    public SwipeHandlerTangent(CandySwipeSettings swipeSettings, Board board)
    {
        this.swipeSettings = swipeSettings;
        this.board = board;
    }
    public void HandleSwipe(Vector2 touchPosition, Vector2 releasePosition, Candy swappedCandy)
    {
        if (Vector2.Distance(touchPosition, releasePosition) > swipeSettings.slideOffset)
        {
            Vector2 swipeDirection = GetSwipeDirection(touchPosition, releasePosition);
            board.ControlSwipe(swappedCandy, swipeDirection);
        }
    }
    private  Vector2 GetSwipeDirection(Vector2 first, Vector2 second)
    {
        float swipeAngle = CalculateTangent(first, second);
        Vector2 direction = GetTangentResult(swipeAngle);
        return direction;
    }
    private  float CalculateTangent(Vector2 first, Vector2 second)
    {
        float swipeAngle = Mathf.Atan2(second.y - first.y, second.x - first.x);
        swipeAngle *= Mathf.Rad2Deg;
        return swipeAngle;
    }
    private  Vector2 GetTangentResult(float swipeAngle)
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            return Vector2.right;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            return Vector2.up;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.down;
        }
    }
}
