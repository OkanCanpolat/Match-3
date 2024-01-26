using TMPro;
using UnityEngine;

public class MoveLoseCondition : LoseCondition
{
    public MoveLoseCondition(int counter, TMP_Text counterText, TMP_Text counterNameText) : base(counter, counterText, counterNameText)
    {
        counterName = "MOVE";
    }

    public override void Init(LoseContionController controller)
    {
        counterNameText.text = counterName;
        Board.Instance.OnCandySwapped += OnCandySwap;
        this.controller = controller;
        counterText.text = counter.ToString();
    }

    private void OnCandySwap()
    {
        counter--;
        counterText.text = counter.ToString();
        ControlCounter();
    }
    private void ControlCounter()
    {
        if (counter <= 0)
        {
            Board.Instance.OnCandySwapped -= OnCandySwap;
            controller.Lose();
        }
    }
}
