using UnityEngine;

public class WrappedCandy : Candy
{
    public override void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        board.DestroyCandyAt(CandyProperties.Column, CandyProperties.Row);
        matchMaker.MatchAdjacentCandies(CandyProperties.Column, CandyProperties.Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
}
