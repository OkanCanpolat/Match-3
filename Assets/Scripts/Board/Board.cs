using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    CanMove, CanNotMove
}
public class Board : MonoBehaviour
{
    public static Board Instance;
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
    [SerializeField] private float spawnOffset;
    [SerializeField] private Piece[] columns;
    [SerializeField] private GameObject[] candiesToSpawn;
    [SerializeField] private TileContainer[] tilesColumn;
    private List<CandyPrefabInstantiateDTO> multiMatchCandies = new List<CandyPrefabInstantiateDTO>();
    private bool isChocolateDestroyed;
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public Piece[] Columns { get => columns; }
    public TileContainer[] TilesColumn { get => tilesColumn; }
    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }
    private void Start()
    {
        SetUpRowAndColumnOnAwake();
    }
    public void ControlSwap(Candy candy, float angle)
    {
        int column = candy.Column;
        int row = candy.Row;

        Candy otherCandy = null;
        OnSwap?.Invoke();

        if (angle > -45 && angle <= 45 && column < width - 1 && IsLegalMovementTile(column + 1, row)
            && IsLegalMovementTile(column, row))
        {
            State = BoardState.CanNotMove;

            otherCandy = columns[column + 1].rows[row].GetComponent<Candy>();
            candy.otherCandy = otherCandy.gameObject;

            columns[column].rows[row] = otherCandy.gameObject;
            columns[otherCandy.Column].rows[otherCandy.Row] = candy.gameObject;
            candy.previousColumn = column;
            candy.previousRow = row;
            otherCandy.previousColumn = otherCandy.Column;
            otherCandy.previousRow = otherCandy.Row;

            otherCandy.Column -= 1;
            candy.Column += 1;
            candy.Swap();
        }

        else if (angle > 45 && angle <= 135 && row < height - 1 && IsLegalMovementTile(column, row + 1)
            && IsLegalMovementTile(column, row))
        {
            State = BoardState.CanNotMove;

            otherCandy = columns[column].rows[row + 1].GetComponent<Candy>();
            candy.otherCandy = otherCandy.gameObject;

            columns[column].rows[row] = otherCandy.gameObject;
            columns[otherCandy.Column].rows[otherCandy.Row] = candy.gameObject;
            candy.previousColumn = column;
            candy.previousRow = row;
            otherCandy.previousColumn = otherCandy.Column;
            otherCandy.previousRow = otherCandy.Row;

            otherCandy.Row -= 1;
            candy.Row += 1;
            candy.Swap();
        }

        else if ((angle > 135 || angle <= -135) && column > 0 && IsLegalMovementTile(column - 1, row)
            && IsLegalMovementTile(column, row))
        {
            State = BoardState.CanNotMove;

            otherCandy = columns[column - 1].rows[row].GetComponent<Candy>();
            candy.otherCandy = otherCandy.gameObject;

            columns[column].rows[row] = otherCandy.gameObject;
            columns[otherCandy.Column].rows[otherCandy.Row] = candy.gameObject;
            candy.previousColumn = column;
            candy.previousRow = row;
            otherCandy.previousColumn = otherCandy.Column;
            otherCandy.previousRow = otherCandy.Row;

            otherCandy.Column += 1;
            candy.Column -= 1;
            candy.Swap();
        }
        else if (angle < -45 && angle >= -135 && row > 0 && IsLegalMovementTile(column, row - 1)
            && IsLegalMovementTile(column, row))
        {
            State = BoardState.CanNotMove;

            otherCandy = columns[column].rows[row - 1].GetComponent<Candy>();
            candy.otherCandy = otherCandy.gameObject;

            columns[column].rows[row] = otherCandy.gameObject;
            columns[otherCandy.Column].rows[otherCandy.Row] = candy.gameObject;
            candy.previousColumn = column;
            candy.previousRow = row;
            otherCandy.previousColumn = otherCandy.Column;
            otherCandy.previousRow = otherCandy.Row;

            otherCandy.Row += 1;
            candy.Row -= 1;
            candy.Swap();
        }
    }
    public void ControlMatches()
    {
        StartCoroutine(ControlMatchesCo());
    }
    public IEnumerator ControlMatchesCo()
    {
        State = BoardState.CanNotMove;

        yield return new WaitForSeconds(.4f);

        for (int i = 0; i < width; i++)
        {
            Piece piece = columns[i];

            for (int j = 0; j < height; j++)
            {
                GameObject go = piece.rows[j];
                Candy candy = go != null ? go.GetComponent<Candy>() : null;

                if (candy != null && candy.isMatched)
                {
                    candy.OnMatched();
                }
            }
        }

        CreateMultiMatchCandies();
        StartCoroutine(DecreaseRows());
    }
    public void DestroyCandyAt(int column, int row)
    {
        GameObject candy = columns[column].rows[row];

        if (candy != null)
        {
            columns[column].rows[row] = null;
            Destroy(candy);
            ControlTileReaction(column, row);
            OnDestroy?.Invoke();
        }
    }
    public void MatchColumnCandies(int column, int row)
    {
        Piece piece = columns[column];

        for (int i = 0; i < height; i++)
        {
            if (i != row && piece.rows[i] != null)
            {
                Candy candy = piece.rows[i].GetComponent<Candy>();
                candy.isMatched = true;
                candy.OnMatched();
            }
        }
    }
    public void MatchRowCandies(int column, int row)
    {
        for (int i = 0; i < width; i++)
        {
            if (i != column && columns[i].rows[row] != null)
            {
                Candy candy = columns[i].rows[row].GetComponent<Candy>();
                candy.isMatched = true;
                candy.OnMatched();
            }
        }
    }
    public void MatchAdjacentCandies(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > height - 1) continue;

                if (columns[i].rows[j] != null)
                {
                    Candy candy = columns[i].rows[j].GetComponent<Candy>();
                    candy.isMatched = true;
                    candy.OnMatched();
                }
            }
        }
    }
    public void MatchAllCandiesWithTypeOf(CandyType type)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (columns[i].rows[j] != null)
                {
                    Candy candy = columns[i].rows[j].GetComponent<Candy>();
                    MatchCandyOfType(candy, type);
                }
            }
        }
    }
    public bool NeitherEmptyNorBlank(int column, int row)
    {
        if (columns[column].rows[row] != null && tilesColumn[column].rows[row].Type != TileType.Blank)
        {
            return true;
        }
        return false;
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
    public void SwitchPieces(int column, int row, Vector2 direction)
    {
        Candy sourceCandy = columns[column].rows[row].GetComponent<Candy>();
        GameObject holder = columns[column + (int)direction.x].rows[row + (int)direction.y];
        Candy holderCandy = holder.GetComponent<Candy>();
        columns[column + (int)direction.x].rows[row + (int)direction.y] = columns[column].rows[row];
        columns[column].rows[row] = holder;
        int tempColumn = sourceCandy.Column;
        int tempRow = sourceCandy.Row;
        sourceCandy.Column = holderCandy.Column;
        sourceCandy.Row = holderCandy.Row;
        holderCandy.Column = tempColumn;
        holderCandy.Row = tempRow;
    }
    public void InstantiateCandyAt(GameObject candyToInstantiate, Candy candy)
    {
        StartCoroutine(InstantiateCandyAtCo(candyToInstantiate, candy));
    }
    public void AddMultiMatchCandy(GameObject candy, int column, int row)
    {
        Candy matchedCandy = columns[column].rows[row].GetComponent<Candy>();
        matchedCandy.Type = CandyType.Destroyed;
        CandyPrefabInstantiateDTO createdCandy = new CandyPrefabInstantiateDTO(candy, column, row);
        multiMatchCandies.Add(createdCandy);
    }
    public void CandySwapped()
    {
        OnCandySwapped?.Invoke();
    }
    private bool IsEmpty(int column, int row)
    {
        if (tilesColumn[column].rows[row] == null) return true;
        return false;
    }
    private bool IsBlank(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Blank) return true;
        return false;
    }
    private bool IsLocked(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Locked) return true;
        return false;
    }
    private bool IsBox(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Box) return true;
        return false;
    }
    private bool IsChocolate(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type == TileType.Chocolate) return true;
        return false;
    }
    private void CreateMultiMatchCandies()
    {
        foreach (CandyPrefabInstantiateDTO dto in multiMatchCandies)
        {
            Vector2 position = new Vector2(dto.Column, dto.Row);
            GameObject createdObject = Instantiate(dto.Candy, position, Quaternion.identity);
            Candy createdCandy = createdObject.GetComponent<Candy>();
            createdCandy.Column = dto.Column;
            createdCandy.Row = dto.Row;
            columns[dto.Column].rows[dto.Row] = createdObject;
        }

        multiMatchCandies.Clear();
    }
    private IEnumerator DecreaseRows()
    {
        List<Candy> candies = new List<Candy>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (columns[i].rows[j] == null && IsLegalMovementTile(i, j))
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (columns[i].rows[k] != null)
                        {
                            Candy candy = columns[i].rows[k].GetComponent<Candy>();
                            candy.Row = j;
                            columns[i].rows[k] = null;
                            columns[i].rows[j] = candy.gameObject;
                            candy.MovePiece();
                            candies.Add(candy);
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(.4f);
        RefillBoard(candies);

        if (FindMatchesOf(candies))
        {
            ControlMatches();
        }
        else
        {
            ControlChocolateSpread();
            State = BoardState.CanMove;
        }
    }
    private IEnumerator InstantiateCandyAtCo(GameObject candyToInstantiate, Candy candy)
    {
        yield return new WaitForSeconds(.4f);
        GameObject spawnObject = Instantiate(candyToInstantiate, candy.transform.position, Quaternion.identity);
        Candy createdCandy = spawnObject.GetComponent<Candy>();
        createdCandy.Row = candy.Row;
        createdCandy.Column = candy.Column;
        columns[candy.Column].rows[candy.Row].GetComponent<Candy>().OnMatched();
        columns[candy.Column].rows[candy.Row] = spawnObject;
    }
    private bool FindMatchesOf(List<Candy> candies)
    {
        bool anyMatch = false;

        foreach (Candy candy in candies)
        {
            anyMatch = candy.FindMatch() | anyMatch;
        }

        return anyMatch;
    }
    private void RefillBoard(List<Candy> candies)
    {

        for (int i = 0; i < width; i++)
        {
            Piece piece = columns[i];

            for (int j = 0; j < height; j++)
            {
                if (piece.rows[j] == null && IsLegalCreateTile(i, j))
                {
                    Vector2 position = new Vector2(i, j + spawnOffset);
                    int spawnIndex = UnityEngine.Random.Range(0, candiesToSpawn.Length);
                    GameObject spawnObject = Instantiate(candiesToSpawn[spawnIndex], position, Quaternion.identity);
                    Candy candy = spawnObject.GetComponent<Candy>();
                    columns[i].rows[j] = spawnObject;
                    candy.Row = j;
                    candy.Column = i;
                    candy.MovePiece();
                    candies.Add(candy);
                }
            }
        }
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
                    candy.Column = i;
                    candy.Row = j;
                }
            }
        }
    }
    private void MatchCandyOfType(Candy candy, CandyType type)
    {
        if (candy.Type == type)
        {
            candy.isMatched = true;
        }
    }
    private void ControlTileReaction(int column, int row)
    {
        ControlBreakableTile(column, row);
        ControlLockedTile(column, row);
        ControlBoxTile(column, row);
        ControlChocolateTile(column, row);
    }
    private void ControlBreakableTile(int column, int row)
    {
        if (tilesColumn[column].rows[row].Type != TileType.Breakable) return;

        DestroyReactiveTile breakbleTile = tilesColumn[column].rows[row].GetComponent<DestroyReactiveTile>();

        if (breakbleTile.ReactToDestroy())
        {
            Vector2 position = new Vector2(column, row);
            GameObject tile1 = Instantiate(basicTile, position, Quaternion.identity);
            tilesColumn[column].rows[row] = tile1.GetComponent<BackgroundTile>();
        }
    }
    private void ControlLockedTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > height - 1) continue;

                if (tilesColumn[i].rows[j].Type == TileType.Locked)
                {
                    Destroy(tilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Instantiate(basicTile, position, Quaternion.identity);
                    tilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }
    private void ControlBoxTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > height - 1) continue;

                if (tilesColumn[i].rows[j].Type == TileType.Box)
                {
                    Destroy(tilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Instantiate(basicTile, position, Quaternion.identity);
                    tilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }
    private void ControlChocolateTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > height - 1) continue;

                if (tilesColumn[i].rows[j].Type == TileType.Chocolate)
                {
                    isChocolateDestroyed = true;
                    Destroy(tilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Instantiate(basicTile, position, Quaternion.identity);
                    tilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }
    private void ControlChocolateSpread()
    {
        if (isChocolateDestroyed)
        {
            isChocolateDestroyed = false;
        }
        else
        {
            GenerateChocolateTile();

        }
    }
    private List<BackgroundTile> GetChocolateTiles()
    {
        List<BackgroundTile> chocolateTiles = new List<BackgroundTile>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilesColumn[i].rows[j].Type == TileType.Chocolate)
                {
                    chocolateTiles.Add(tilesColumn[i].rows[j]);
                }
            }
        }

        return chocolateTiles;
    }
    private List<BackgroundTile> GetAdjecentTiles(int column, int row)
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
    private Vector2 GetTileColumnRow(BackgroundTile tile)
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
    private void GenerateChocolateTile()
    {
        List<BackgroundTile> chocolateTiles = GetChocolateTiles();
        chocolateTiles.Shuffle();

        for (int i = 0; i < chocolateTiles.Count; i++)
        {
            BackgroundTile selectedTile = chocolateTiles[i];
            Vector2 tileColumnRow = GetTileColumnRow(selectedTile);
            List<BackgroundTile> adjecentTiles = GetAdjecentTiles((int)tileColumnRow.x, (int)tileColumnRow.y);
            adjecentTiles.Shuffle();

            for (int j = 0; j < adjecentTiles.Count; j++)
            {

                if (adjecentTiles[j].Type != TileType.Chocolate)
                {
                    Vector2 tilePos = GetTileColumnRow(adjecentTiles[j]);
                    Destroy(tilesColumn[(int)tilePos.x].rows[(int)tilePos.y].gameObject);
                    GameObject tile = Instantiate(chocolateTile, tilePos, Quaternion.identity);
                    tilesColumn[(int)tilePos.x].rows[(int)tilePos.y] = tile.GetComponent<BackgroundTile>();

                    Destroy(columns[(int)tilePos.x].rows[(int)tilePos.y].gameObject);
                    columns[(int)tilePos.x].rows[(int)tilePos.y] = null;
                    return;
                }
            }

        }
    }
}
