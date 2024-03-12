using System.Collections;
using UnityEngine;

public class ColorBomb : Candy
{
    public override void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        board.DestroyCandyAt(CandyProperties.Column, CandyProperties.Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
    public override void Swap()
    {
        StartCoroutine(SwapCo());
    }
    private IEnumerator SwapCo()
    {
        Candy other = CandyProperties.OtherCandy.GetComponent<Candy>();
        other.MovePiece();

        yield return StartCoroutine(MovePieceCo());

        isMatched = true;
        board.OnCandySwap();
        matchMaker.MatchAllCandiesWithTypeOf(other.Type);
        board.ControlMatches();
    }
}
