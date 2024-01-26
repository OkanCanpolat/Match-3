using System.Collections;
using UnityEngine;

public class ColorBomb : Candy
{
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
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
    public override bool FindMatch(bool controlMatchCount = true)
    {
        return false;
    }
    public override void Swap()
    {
        StartCoroutine(SwapCo());
    }
    private IEnumerator SwapCo()
    {
        Candy other = otherCandy.GetComponent<Candy>();
        other.MovePiece();

        yield return StartCoroutine(MovePieceCo());

        isMatched = true;
        Board.Instance.CandySwapped();
        Board.Instance.MatchAllCandiesWithTypeOf(other.Type);
        Board.Instance.ControlMatches();
    }
}
