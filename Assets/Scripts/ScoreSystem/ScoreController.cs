using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider startSlider;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private ScorCounter counter;

    private void Awake()
    {
        counter.OnScoreChanged += ChangeScoreText;
        counter.OnStartChanged += ChangeStarImagesAndBar;
    }
    private void ChangeScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
    private void ChangeStarImagesAndBar(int star)
    {
        float barFillAmount = (float)star / stars.Length;
        startSlider.value = barFillAmount;
        
        for(int i = 0; i < star; i++)
        {
            stars[i].SetActive(true);
        }
    }
}
