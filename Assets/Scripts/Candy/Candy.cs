using System.Collections;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [HideInInspector] public int Row;
    [HideInInspector] public int Column;
    [HideInInspector] public int previousRow;
    [HideInInspector] public int previousColumn;
    [HideInInspector] public bool isMatched;
    [HideInInspector] public GameObject otherCandy = null;
    public CandyType Type;
    public EventBusType EventType;
    public GameObject destroyParticlePrefab;
    private Vector3 touchPosition;
    private Vector3 releasePosition;
    private float swipeAngle;
    private static float slideSpeed = 0.1f;
    private static float slideOffset = 0.4f;
    private MatchesDTO matches;

    public virtual void Awake()
    {
        matches = new MatchesDTO(this);
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
        Candy other = otherCandy.GetComponent<Candy>();
        other.MovePiece();

        yield return StartCoroutine(MovePieceCo());

        FindMatch();
        other.FindMatch();

        if (!other.isMatched && !isMatched)
        {
            other.MoveBackPiece();
            yield return StartCoroutine(MoveBackCo());
            Board.Instance.State = BoardState.CanMove;
        }
        else
        {
            Board.Instance.CandySwapped();
            Board.Instance.ControlMatches();
        }
    }
    public virtual bool FindMatch(bool controlMatchCount = true)
    {
        bool Horizontalmatch = FindHorizontalMatch();
        bool Verticalmatch = FindVerticalMatch();

        if ((Horizontalmatch || Verticalmatch) && controlMatchCount)
        {
            MatchCounter.Instance.CountMatches(matches);
        }

        return Horizontalmatch | Verticalmatch;
    }
    public virtual void OnMatched()
    {
        SoundManager.Instance.PlayCandyDestroySound();
        EventBus.Publish(EventType);
        Board.Instance.DestroyCandyAt(Column, Row);
        Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);
    }
    public virtual void OnMouseDown()
    {
        if (!(Board.Instance.State == BoardState.CanMove) || GameManager.Instance.GameState == GameState.Finish) return;
        touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public virtual void OnMouseUp()
    {
        if (!(Board.Instance.State == BoardState.CanMove) || GameManager.Instance.GameState == GameState.Finish) return;
        releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(touchPosition, releasePosition) > slideOffset)
        {
            CalculateAngle();
            Board.Instance.ControlSwap(this, swipeAngle);
        }
    }
    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(releasePosition.y - touchPosition.y, releasePosition.x - touchPosition.x);
        swipeAngle *= Mathf.Rad2Deg;
    }
    protected IEnumerator MovePieceCo()
    {
        float t = 0;

        Vector2 destination = new Vector2(Column, Row);
        Vector2 currentPostion = transform.position;

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / slideSpeed;
            yield return null;
        }

        transform.position = destination;
    }
    protected IEnumerator MoveBackCo()
    {
        float t = 0;
        Vector2 destination = new Vector2(previousColumn, previousRow);
        Vector2 currentPostion = transform.position;

        Column = previousColumn;
        Row = previousRow;

        Board.Instance.Columns[Column].rows[Row] = gameObject;

        while (t < 1)
        {
            transform.position = Vector2.Lerp(currentPostion, destination, t);
            t += Time.deltaTime / slideSpeed;
            yield return null;
        }

        transform.position = destination;
        otherCandy = null;
    }
    private bool FindHorizontalMatch()
    {
        bool matched = false;

        Candy right1 = null;
        Candy left1 = null;
        Candy right2 = null;
        Candy left2 = null;

        if (Column < Board.Instance.Width - 1 && Board.Instance.Columns[Column + 1].rows[Row] != null)
        {
            right1 = Board.Instance.Columns[Column + 1].rows[Row].GetComponent<Candy>();

            if (right1.GetComponent<Candy>().Column < Board.Instance.Width - 1 &&
                Board.Instance.Columns[Column + 2].rows[Row] != null)
            {
                right2 = Board.Instance.Columns[Column + 2].rows[Row].GetComponent<Candy>();
            }
        }

        if (Column > 0 && Board.Instance.Columns[Column - 1].rows[Row] != null)
        {
            left1 = Board.Instance.Columns[Column - 1].rows[Row].GetComponent<Candy>();

            if (left1.GetComponent<Candy>().Column > 0 && Board.Instance.Columns[Column - 2].rows[Row] != null)
            {
                left2 = Board.Instance.Columns[Column - 2].rows[Row].GetComponent<Candy>();
            }
        }


        if (right1 != null && left1 != null && right1.Type == Type && left1.Type == Type)
        {
            matched = true;
            right1.isMatched = true;
            left1.isMatched = true;
            isMatched = true;
        }

        if (right1 != null && right2 != null && right1.Type == Type && right2.Type == Type)
        {
            matched = true;
            right1.isMatched = true;
            right2.isMatched = true;
            isMatched = true;
        }
        if (left1 != null && left2 != null && left1.Type == Type && left2.Type == Type)
        {
            matched = true;
            left1.isMatched = true;
            left2.isMatched = true;
            isMatched = true;
        }

        matches.InitHorizontalMatches(right1, right2, left1, left2);
        return matched;
    }
    private bool FindVerticalMatch()
    {
        bool matched = false;

        Candy up1 = null;
        Candy down1 = null;
        Candy up2 = null;
        Candy down2 = null;

        if (Row < Board.Instance.Height - 1 && Board.Instance.Columns[Column].rows[Row + 1] != null)
        {
            up1 = Board.Instance.Columns[Column].rows[Row + 1].GetComponent<Candy>();

            if (up1.GetComponent<Candy>().Row < Board.Instance.Height - 1 && Board.Instance.Columns[Column].rows[Row + 2] != null)
            {
                up2 = Board.Instance.Columns[Column].rows[Row + 2].GetComponent<Candy>();
            }
        }
        if (Row > 0 && Board.Instance.Columns[Column].rows[Row - 1] != null)
        {
            down1 = Board.Instance.Columns[Column].rows[Row - 1].GetComponent<Candy>();

            if (down1.GetComponent<Candy>().Row > 0 && Board.Instance.Columns[Column].rows[Row - 2] != null)
            {
                down2 = Board.Instance.Columns[Column].rows[Row - 2].GetComponent<Candy>();
            }
        }

        if (up1 != null && down1 != null && up1.Type == Type && down1.Type == Type)
        {
            matched = true;
            up1.isMatched = true;
            down1.isMatched = true;
            isMatched = true;
        }

        if (up1 != null && up2 != null && up1.Type == Type && up2 != null && up2.Type == Type)
        {
            matched = true;
            up1.isMatched = true;
            up2.isMatched = true;
            isMatched = true;
        }
        if (down1 != null && down2 != null && down1.Type == Type && down2 != null && down2.Type == Type)
        {
            matched = true;
            down1.isMatched = true;
            down2.isMatched = true;
            isMatched = true;
        }

        matches.InitVerticalMatches(up1, up2, down1, down2);
        return matched;
    }
}
