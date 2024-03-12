using System;
using UnityEngine;
using Zenject;

public class ScorCounter : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnStartChanged;
    [SerializeField] private int scorePerDestroy;
    [SerializeField] private int maxScore;
    [SerializeField] private int scoreToOneStar;
    [SerializeField] private int maxStarCount;
    private Board board;
    private int currentScore;
    private int currentStarCount = 0;
    public int CurrentStartCount { get => currentStarCount; }
    public int CurrentScore { get => currentScore; }

    [Inject]
    public void Construct(Board board)
    {
        this.board = board;
    }
    private void Start()
    {
        board.OnDestroy += OnCandyDestroyed;
        OnScoreChanged?.Invoke(currentScore);
    }
    private void OnCandyDestroyed()
    {
        currentScore += scorePerDestroy;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        OnScoreChanged?.Invoke(currentScore);
        ControlStarCount();
    }
    private void ControlStarCount()
    {
        int lastStarCount = currentStarCount;
        currentStarCount = currentScore / scoreToOneStar;
        currentStarCount = Mathf.Clamp(currentStarCount, 0, maxStarCount);

        if (currentStarCount != lastStarCount)
        {
            OnStartChanged?.Invoke(currentStarCount);
        }
    }
}
