using System.Collections.Generic;
using UnityEngine;

public class GoalUIManager : MonoBehaviour
{
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private GameObject goalTemplate;
    [SerializeField] private Transform goalParent;
    private Dictionary<Goal, GoalTemplate> goalTemplateMap = new Dictionary<Goal, GoalTemplate>();

    private void Awake()
    {
        goalManager.OnGoalAdded += OnGoalAdded;
        goalManager.OnGoalAmountChanged += OnGoalAmountChanged;
    }

    private void OnGoalAdded(Goal goal)
    {
        GameObject createdGoal = Instantiate(goalTemplate, goalParent);
        GoalTemplate template = createdGoal.GetComponent<GoalTemplate>();
        template.Init(goal.CurrentAmount, goal.GoalInformation.GoalSprite);
        goalTemplateMap.Add(goal, template);
    }
    private void OnGoalAmountChanged(Goal goal)
    {
        GoalTemplate template = goalTemplateMap[goal];
        template.ChangeText(goal.CurrentAmount);
    }
}
