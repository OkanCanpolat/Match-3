using UnityEngine;
public class ColumnBombCandy : Candy
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
        Board.Instance.MatchColumnCandies(Column, Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);


    }

}
