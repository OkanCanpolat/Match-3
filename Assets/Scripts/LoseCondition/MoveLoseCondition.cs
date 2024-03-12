using TMPro;
using UnityEngine;
using Zenject;

public class MoveLoseCondition : LoseConditionBase
{
    Board board;
    public MoveLoseCondition(int counter, TMP_Text counterText, TMP_Text counterNameText) : base(counter, counterText, counterNameText)
    {
        counterName = "MOVE";
    }

    [Inject]
    public void Construct(Board board)
    {
        this.board = board;
    }

    public override void Init(LoseContionController controller)
    {
        counterNameText.text = counterName;
        board.OnCandySwapped += OnCandySwap;
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
            board.OnCandySwapped -= OnCandySwap;
            controller.Lose();
        }
    }
}
