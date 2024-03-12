using System.Collections;
using UnityEngine;
using Zenject;
public struct CandyProperties
{
    public int Row;
    public int Column;
    public int PreviousRow;
    public int PreviousColumn;
    public GameObject OtherCandy;
    public Vector2 TouchPosition;
    public Vector2 ReleasePosition;
}
public class Candy : MonoBehaviour
{
    public CandyProperties CandyProperties;
    [HideInInspector] public bool isMatched;
    public CandyType Type;
    public EventBusType EventType;
    public GameObject destroyParticlePrefab;
    private CandySwipeSettings candySwipeSettings;
    private MatchesDTO matches;
    private ISwipeHandler swipeHandler;
    private IMatchFinder matchFinder;
    protected IMatchMaker matchMaker;
    protected Board board;

    [Inject]
    public void Construct(IMatchFinder matchFinder, IMatchMaker matchMaker, ISwipeHandler swipeHandler,
                                CandySwipeSettings candySwipeSettings, Board board)
    {
        this.matchFinder = matchFinder;
        this.candySwipeSettings = candySwipeSettings;
        this.swipeHandler = swipeHandler;
        this.board = board;
        this.matchMaker = matchMaker;
        matches = new MatchesDTO(this);
    }
    public void OnMouseDown()
    {
        if (!(board.State == BoardState.CanMove) || GameManager.Instance.GameState == GameState.Finish) return;
        CandyProperties.TouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void OnMouseUp()
    {
        if (!(board.State == BoardState.CanMove) || GameManager.Instance.GameState == GameState.Finish) return;
        CandyProperties.ReleasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        swipeHandler.HandleSwipe(CandyProperties.TouchPosition, CandyProperties.ReleasePosition, this);
    }
    public void MovePiece()
    {
        StartCoroutine(MovePieceCo());
    }
    public void MoveBackPiece()
    {
        StartCoroutine(MoveBackCo());
    }
    public virtual void Swap()
    {
        StartCoroutine(SwapCo());
    }
    private IEnumerator SwapCo()
    {
        Candy other = CandyProperties.OtherCandy.GetComponent<Candy>();
        other.MovePiece();

        yield return StartCoroutine(MovePieceCo());

        if (FindMatch() | other.FindMatch())
        {
            board.OnCandySwap();
            board.ControlMatches();
        }
        else
        {
            other.MoveBackPiece();
            yield return StartCoroutine(MoveBackCo());
            board.State = BoardState.CanMove;
        }
    }
    public virtual bool FindMatch(bool markMatches = true)
    {
        return matchFinder.FindMatch(this, matches, markMatches);
    }
    public virtual void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        board.DestroyCandyAt(CandyProperties.Column, CandyProperties.Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
    protected IEnumerator MovePieceCo()
    {
        float t = 0;

        Vector2 destination = new Vector2(CandyProperties.Column, CandyProperties.Row);
        Vector2 currentPostion = transform.position;

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / candySwipeSettings.slideSpeed;
            yield return null;
        }

        transform.position = destination;
    }
    protected IEnumerator MoveBackCo()
    {
        float t = 0;
        Vector2 destination = new Vector2(CandyProperties.PreviousColumn, CandyProperties.PreviousRow);
        Vector2 currentPostion = transform.position;

        CandyProperties.Column = CandyProperties.PreviousColumn;
        CandyProperties.Row = CandyProperties.PreviousRow;

        board.Columns[CandyProperties.Column].rows[CandyProperties.Row] = gameObject;

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / candySwipeSettings.slideSpeed;
            yield return null;
        }

        transform.position = destination;
        CandyProperties.OtherCandy = null;
    }
}
