using UnityEngine;

public class WrappedCandy : Candy
{
    public override void Awake()
    {
        base.Awake();
    }
    public override void OnMouseDown()
    {
        base.OnMouseDown();
    }
    public override void OnMouseUp()
    {
        base.OnMouseUp();
    }
    public override void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        Board.Instance.DestroyCandyAt(Column, Row);
        Board.Instance.MatchAdjacentCandies(Column, Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
}
