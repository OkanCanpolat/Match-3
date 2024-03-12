using UnityEngine;
public class ColumnBombCandy : Candy
{
    public override void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        board.DestroyCandyAt(CandyProperties.Column, CandyProperties.Row);
        matchMaker.MatchColumnCandies(CandyProperties.Column, CandyProperties.Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
}
