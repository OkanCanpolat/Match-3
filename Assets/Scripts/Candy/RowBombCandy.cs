using UnityEngine;
public class RowBombCandy : Candy
{
    public override void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        board.DestroyCandyAt(CandyProperties.Column, CandyProperties.Row);
        matchMaker.MatchRowCandies(CandyProperties.Column, CandyProperties.Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
}
