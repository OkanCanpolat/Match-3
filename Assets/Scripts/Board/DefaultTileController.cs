using System.Collections.Generic;
using UnityEngine;

public class DefaultTileController : ITileController
{
    private Board board;
    private GameObject basicTile;
    private GameObject chocolateTile;
    private bool isChocolateDestroyed;
    public DefaultTileController(Board board, GameObject basicTile, GameObject chocolateTile)
    {
        this.board = board;
        this.basicTile = basicTile;
        this.chocolateTile = chocolateTile;
    }
    public void ControlTileReactions(int column, int row)
    {
        ControlBreakableTile(column, row);
        ControlLockedTile(column, row);
        ControlBoxTile(column, row);
        ControlChocolateTile(column, row);
    }
    public void ControlTileSpreads()
    {
        ControlChocolateSpread();
    }
    private void ControlBreakableTile(int column, int row)
    {
        if (board.TilesColumn[column].rows[row].Type != TileType.Breakable) return;

        DestroyReactiveTile breakbleTile = board.TilesColumn[column].rows[row].GetComponent<DestroyReactiveTile>();

        if (breakbleTile.ReactToDestroy())
        {
            Vector2 position = new Vector2(column, row);
            GameObject tile1 = Object.Instantiate(basicTile, position, Quaternion.identity);
            board.TilesColumn[column].rows[row] = tile1.GetComponent<BackgroundTile>();
        }
    }
    private void ControlLockedTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > board.Width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > board.Height - 1) continue;

                if (board.TilesColumn[i].rows[j].Type == TileType.Locked)
                {
                    Object.Destroy(board.TilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Object.Instantiate(basicTile, position, Quaternion.identity);
                    board.TilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }
    private void ControlBoxTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > board.Width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > board.Height - 1) continue;

                if (board.TilesColumn[i].rows[j].Type == TileType.Box)
                {
                    Object.Destroy(board.TilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Object.Instantiate(basicTile, position, Quaternion.identity);
                    board.TilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }
    private void ControlChocolateTile(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            if (i < 0 || i > board.Width - 1) continue;

            for (int j = row - 1; j <= row + 1; j++)
            {
                if (j < 0 || j > board.Height - 1) continue;

                if (board.TilesColumn[i].rows[j].Type == TileType.Chocolate)
                {
                    isChocolateDestroyed = true;
                    Object.Destroy(board.TilesColumn[i].rows[j].gameObject);
                    Vector2 position = new Vector2(i, j);
                    GameObject tile = Object.Instantiate(basicTile, position, Quaternion.identity);
                    board.TilesColumn[i].rows[j] = tile.GetComponent<BackgroundTile>();
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
    private void GenerateChocolateTile()
    {
        List<BackgroundTile> chocolateTiles = GetChocolateTiles();
        chocolateTiles.Shuffle();

        for (int i = 0; i < chocolateTiles.Count; i++)
        {
            BackgroundTile selectedTile = chocolateTiles[i];
            Vector2 tileColumnRow = board.GetTilePosition(selectedTile);
            List<BackgroundTile> adjecentTiles = board.GetAdjecentTiles((int)tileColumnRow.x, (int)tileColumnRow.y);
            adjecentTiles.Shuffle();

            for (int j = 0; j < adjecentTiles.Count; j++)
            {

                if (adjecentTiles[j].Type != TileType.Chocolate)
                {
                    Vector2 tilePos = board.GetTilePosition(adjecentTiles[j]);
                    Object.Destroy(board.TilesColumn[(int)tilePos.x].rows[(int)tilePos.y].gameObject);
                    GameObject tile = Object.Instantiate(chocolateTile, tilePos, Quaternion.identity);
                    board.TilesColumn[(int)tilePos.x].rows[(int)tilePos.y] = tile.GetComponent<BackgroundTile>();

                    Object.Destroy(board.Columns[(int)tilePos.x].rows[(int)tilePos.y].gameObject);
                    board.Columns[(int)tilePos.x].rows[(int)tilePos.y] = null;
                    return;
                }
            }
        }
    }
    private List<BackgroundTile> GetChocolateTiles()
    {
        List<BackgroundTile> chocolateTiles = new List<BackgroundTile>();

        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (board.TilesColumn[i].rows[j].Type == TileType.Chocolate)
                {
                    chocolateTiles.Add(board.TilesColumn[i].rows[j]);
                }
            }
        }
        return chocolateTiles;
    }
}
