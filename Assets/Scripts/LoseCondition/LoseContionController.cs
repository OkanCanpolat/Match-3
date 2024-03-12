using System;
using TMPro;
using UnityEngine;
using Zenject;

public enum LoseConditionType
{
    Time, Move
}
public class LoseContionController : MonoBehaviour
{
    public event Action OnLose;
    [SerializeField] private TMP_Text counterNameText;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private int counter;
    private LoseConditionBase currentLoseCondition;
    private LoseConditionBase.Factory factory;

    [Inject]
    public void Construct(LoseConditionBase.Factory factory)
    {
        this.factory = factory;
    }
    private void Awake()
    {
        currentLoseCondition = factory.Create(counter, counterText, counterNameText);
    }
    public void Lose()
    {
        OnLose?.Invoke();
    }
    private void Start()
    {
        InitLoseCondition();
    }
    private void InitLoseCondition()
    {
        currentLoseCondition.Init(this);
    }
}
