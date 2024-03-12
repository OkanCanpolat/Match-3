using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Board;

public class DefaultBoardLoopController : IBoardLoopController
{
    private Board board;
    private List<Candy> candiesToCheck;
    private float spawnOffset;
    private List<GameObject> candiesToSpawn;
    private MatchCounter matchCounter;
    public DefaultBoardLoopController(Board board, MatchCounter matchCounter, List<GameObject> candiesToSpawn, float spawnOffset)
    {
        this.board = board;
        this.candiesToSpawn = candiesToSpawn;
        this.spawnOffset = spawnOffset;
        this.matchCounter = matchCounter;
        candiesToCheck = new List<Candy>();
    }
    public void HandleMatches()
    {
        for (int i = 0; i < board.Width; i++)
        {
            Piece piece = board.Columns[i];

            for (int j = 0; j < board.Height; j++)
            {
                GameObject go = piece.rows[j];
                Candy candy = go != null ? go.GetComponent<Candy>() : null;

                if (candy != null && candy.isMatched)
                {
                    candy.OnMatched();
                }
            }
        }
    }
    public void DecreaseRows()
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (board.Columns[i].rows[j] == null && board.IsLegalMovementTile(i, j))
                {
                    for (int k = j + 1; k < board.Height; k++)
                    {
                        if (board.Columns[i].rows[k] != null)
                        {
                            Candy candy = board.Columns[i].rows[k].GetComponent<Candy>();
                            candy.CandyProperties.Row = j;
                            board.Columns[i].rows[k] = null;
                            board.Columns[i].rows[j] = candy.gameObject;
                            candy.MovePiece();
                            candiesToCheck.Add(candy);
                            break;
                        }
                    }
                }
            }
        }
    }
    public void RefillBoard()
    {
        for (int i = 0; i < board.Width; i++)
        {
            Piece piece = board.Columns[i];

            for (int j = 0; j < board.Height; j++)
            {
                if (piece.rows[j] == null && board.IsLegalCreateTile(i, j))
                {
                    Vector2 position = new Vector2(i, j + spawnOffset);
                    int spawnIndex = Random.Range(0, candiesToSpawn.Count);
                    GameObject spawnObject = Object.Instantiate(candiesToSpawn[spawnIndex], position, Quaternion.identity);
                    Candy candy = spawnObject.GetComponent<Candy>();
                    board.Columns[i].rows[j] = spawnObject;
                    candy.CandyProperties.Row = j;
                    candy.CandyProperties.Column = i;
                    candy.MovePiece();
                    candiesToCheck.Add(candy);
                }
            }
        }
    }
    public void CreateMultimatchCandies()
    {
        List<CandyPrefabInstantiateDTO> candies = matchCounter.MultiMatchCandies;

        foreach (CandyPrefabInstantiateDTO dto in candies)
        {
            Vector2 position = new Vector2(dto.Column, dto.Row);
            GameObject createdObject = Object.Instantiate(dto.Candy, position, Quaternion.identity);
            Candy createdCandy = createdObject.GetComponent<Candy>();
            createdCandy.CandyProperties.Column = dto.Column;
            createdCandy.CandyProperties.Row = dto.Row;
            board.Columns[dto.Column].rows[dto.Row] = createdObject;
        }
        candies.Clear();
    }
    public bool AnyNewMatch()
    {
        bool anyMatch = false;

        foreach (Candy candy in candiesToCheck)
        {
            anyMatch = candy.FindMatch() | anyMatch;
        }

        candiesToCheck.Clear();
        return anyMatch;
    }
}
