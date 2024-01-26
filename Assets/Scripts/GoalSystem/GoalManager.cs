using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Goal
{
    public GoalInformation GoalInformation;
    public int RequiredAmount;
    public int CurrentAmount;
}
public class GoalManager : MonoBehaviour
{
    public event Action<Goal> OnGoalAdded;
    public event Action<Goal> OnGoalAmountChanged;
    public event Action OnWin;
    [SerializeField] private List<Goal> goals;
    private int finishedGoalCount;

    private void Start()
    {
        SetupGoals();
        SetupEventBusEvents();
    }
    private void SetupGoals()
    {
        finishedGoalCount = 0;

        foreach (Goal goal in goals)
        {
            goal.CurrentAmount = goal.RequiredAmount;
            OnGoalAdded?.Invoke(goal);
        }
    }
    private void ControlGoal(EventBusType type)
    {
        foreach (Goal goal in goals)
        {
            if(goal.GoalInformation.Type == type)
            {
                goal.CurrentAmount--;
                goal.CurrentAmount = Mathf.Clamp(goal.CurrentAmount, 0, goal.RequiredAmount);
                OnGoalAmountChanged(goal);
                ControlGoalState(goal);
                return;
            }
        }
    }
    private void ControlGoalState(Goal goal)
    {
        if(goal.CurrentAmount <= 0)
        {
            EventBus.Unsubscribe(goal.GoalInformation.Type, ControlGoal);
            finishedGoalCount++;
            ControlGameState();
        }
    }
    private void ControlGameState()
    {
        if(finishedGoalCount >= goals.Count)
        {
            OnWin?.Invoke();
        }
    }

    private void SetupEventBusEvents()
    {
        foreach(Goal goal in goals)
        {
            EventBus.Subscribe(goal.GoalInformation.Type, ControlGoal);
        }
    }
}
