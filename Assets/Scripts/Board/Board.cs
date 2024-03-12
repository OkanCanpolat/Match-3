using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum BoardState
{
    CanMove, CanNotMove
}
public class Board : MonoBehaviour
{
    [Serializable]
    public class Piece
    {
        public GameObject[] rows;
    }
    #region TilePrefabs
    [SerializeField] private GameObject basicTile;
    [SerializeField] private GameObject chocolateTile;
    #endregion

    public BoardState State;
    public event Action OnSwap;
    public event Action OnDestroy;
    public event Action OnCandySwapped;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Piece[] columns;
    [SerializeField] private TileContainer[] tilesColumn;
    private ISwipeController swipeController;
    private IBoardLoopController boardLoopController;
    private ITileController tileController;

    #region Getter/Setter
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public Piece[] Columns { get => columns; }
    public TileContainer[] TilesColumn { get => tilesColumn; }
    #endregion
    private void Start()
    {
        SetUpRowAndColumnOnAwake();
    }
    [Inject]
    public void Construct(ISwipeController swipeController, IBoardLoopController boardLoopController, ITileController tileController)
    {
        this.swipeController = swipeController;
        this.boardLoopController = boardLoopController;
        this.tileController = tileController;
    }
    public void ControlSwipe(Candy source, Vector2 swipeDirection)
    {
        swipeController.ControlSwipe(source, swipeDirection);
    }
    public void ControlMatches()
    {
        StartCoroutine(ControlMatchesCo());
    }
    public IEnumerator ControlMatchesCo()
    {
        State = BoardState.CanNotMove;

        yield return new WaitForSeconds(.4f);
        boardLoopController.HandleMatches();
        boardLoopController.CreateMultimatchCandies();
        boardLoopController.DecreaseRows();
        yield return new WaitForSeconds(.4f);
        boardLoopController.RefillBoard();

        if (boardLoopController.AnyNewMatch())
        {
            ControlMatches();
        }
        else
        {
            tileController.ControlTileSpreads();
            State = BoardState.CanMove;
        }
    }
    public void DestroyCandyAt(int column, int row)
    {
        GameObject candy = columns[column].rows[row];

        if (candy != null)
        {
            columns[column].rows[row] = null;
            Destroy(candy);
            tileController.ControlTileReactions(column, row);
            OnDestroy?.Invoke();
        }
    }
    public bool IsLegalMovementTile(int column, int row)
    {
        return !IsEmpty(column, row) && !IsBlank(column, row)
            && !IsLocked(column, row) && !IsBox(column, row) && !IsChocolate(column, row);
    }
    public bool IsLegalCreateTile(int column, int row)
    {
        return !IsEmpty(column, row) && !IsBlank(column, row) && !IsBox(column, row)
            && !IsChocolate(column, row);
    }
    public bool IsInsideBoard(int column, int row)
    {
        return IsColumnInsideBoard(column) && IsRowInsideBoard(row);
    }
    public bool IsColumnInsideBoard(int column)
    {
        return column < width && column >= 0;
    }
    public bool IsRowInsideBoard(int row)
    {
        return row < height && row >= 0;
    }
    public void SwitchPieces(int column, int row, Vector2 direction)
    {
        Candy sourceCandy = columns[column].rows[row].GetComponent<Candy>();
        GameObject holder = columns[column + (int)direction.x].rows[row + (int)direction.y];
        Candy holderCandy = holder.GetComponent<Candy>();
        columns[column + (int)direction.x].rows[row + (int)direction.y] = columns[column].rows[row];
        columns[column].rows[row] = holder;
        int tempColumn = sourceCandy.CandyProperties.Column;
        int tempRow = sourceCandy.CandyProperties.Row;
        sourceCandy.CandyProperties.Column = holderCandy.CandyProperties.Column;
        sourceCandy.CandyProperties.Row = holderCandy.CandyProperties.Row;
        holderCandy.CandyProperties.Column = tempColumn;
        holderCandy.CandyProperties.Row = tempRow;
    }
    public Candy TryGetDrop(int column, int row)
    {
        Candy result;

        if (!IsInsideBoard(column, row) || IsEmpty(column, row) || columns[column].rows[row] == null)
        {
            result = null;
        }
        else
        {
            result = columns[column].rows[row].GetComponent<Candy>();
        }

        return result;
    }
    public void OnCandySwap()
    {
        OnCandySwapped?.Invoke();
    }
    public bool IsEmpty(int column, int row)
    {
        if (tilesColumn[column].rows[row] == null) return true;
        return false;
    }
    public bool IsBlank(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Blank) return true;
        return false;
    }
    public bool IsLocked(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Locked) return true;
        return false;
    }
    public bool IsBox(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Box) return true;
        return false;
    }
    public bool IsChocolate(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Chocolate) return true;
        return false;
    }
    private void SetUpRowAndColumnOnAwake()
    {
        for (int i = 0; i < width; i++)
        {
            Piece piece = columns[i];

            for (int j = 0; j < height; j++)
            {
                if (piece.rows[j] != null)
                {
                    Candy candy = piece.rows[j].GetComponent<Candy>();
                    candy.CandyProperties.Column = i;
                    candy.CandyProperties.Row = j;
                }
            }
        }
    }
    public List<BackgroundTile> GetAdjecentTiles(int column, int row)
    {
        List<BackgroundTile> adjecentTiles = new List<BackgroundTile>();

        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > height - 1) continue;

                if (IsLegalCreateTile(i, j) && tilesColumn[i].rows[j] != null)
                {
                    adjecentTiles.Add(tilesColumn[i].rows[j]);
                }
            }
        }

        return adjecentTiles;
    }
    public Vector2 GetTilePosition(BackgroundTile tile)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilesColumn[i].rows[j] == tile)
                {
                    return new Vector2(i, j);
                }
            }
        }

        return new Vector2(-1, -1);
    }
}
