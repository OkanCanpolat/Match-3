using UnityEngine;

public interface ISwipeHandler 
{
    public void HandleSwipe(Vector2 touchPosition, Vector2 releasePosition, Candy swappedCandy);
}
