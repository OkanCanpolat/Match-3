using TMPro;
using UnityEngine;

public class WinLoseController : MonoBehaviour
{
    [SerializeField] private GameObject winLoseTemplate;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text scorText;
    [SerializeField] private TMP_Text statuText;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private LoseContionController loseManager;
    [SerializeField] private ScorCounter scoreCounter;

    private const string winExplanation = "COMPLETE!";
    private const string loseExplanation = "FAIL!";

    private void Awake()
    {
        loseManager.OnLose += OnLose;
        goalManager.OnWin += OnWin;
    }

    private void OnWin()
    {
        nextButton.SetActive(true);
        winLoseTemplate.SetActive(true);
        ControlStars();
        ControlScore();
        statuText.text = winExplanation;
        GameManager.Instance.GameState = GameState.Finish;
    }
    private void OnLose()
    {
        winLoseTemplate.SetActive(true);
        ControlStars();
        ControlScore();
        statuText.text = loseExplanation;
        GameManager.Instance.GameState = GameState.Finish;
    }
    private void ControlStars()
    {
        for(int i = 0; i< scoreCounter.CurrentStartCount; i++)
        {
            stars[i].SetActive(true);
        }
    }
    private void ControlScore()
    {
        scorText.text = scoreCounter.CurrentScore.ToString();
    }
}
