using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum LoseConditionType
{
    Time, Move
}
public class LoseContionController : MonoBehaviour
{
    public event Action OnLose;
    [SerializeField] private LoseConditionType conditionType;
    [SerializeField] private TMP_Text counterNameText;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private int counter;
    private IDictionary<LoseConditionType, LoseCondition> conditionMap = new Dictionary<LoseConditionType, LoseCondition>();
    private LoseCondition timeLoseCondition;
    private LoseCondition moveLoseCondition;

    private void Awake()
    {
        timeLoseCondition = new TimeLoseCondition(counter, counterText, counterNameText);
        moveLoseCondition = new MoveLoseCondition(counter, counterText, counterNameText);
        conditionMap.Add(LoseConditionType.Time, timeLoseCondition);
        conditionMap.Add(LoseConditionType.Move, moveLoseCondition);
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
        LoseCondition condition = conditionMap[conditionType];
        condition.Init(this);
    }
}
