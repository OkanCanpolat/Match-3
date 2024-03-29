using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Zenject;

public class TimeLoseCondition : LoseConditionBase
{
    public TimeLoseCondition(int counter, TMP_Text counterText, TMP_Text counterNameText) : base(counter, counterText, counterNameText)
    {
        counterName = "TIME";
    }
    public override void Init(LoseContionController controller)
    {
        this.controller = controller;
        counterNameText.text = counterName;
        counterText.text = counter.ToString();
        StartTimer();
    }
    private async Task StartTimer()
    {
        while (counter > 0 && GameManager.Instance.GameState == GameState.Continueu)
        {
            if (Application.isPlaying == false) return;
            await Task.Delay(1000);
            DecreaseTime();
            ControlCounter();
        }
    }
    private void DecreaseTime()
    {
        counter--;
        counterText.text = counter.ToString();
    }
    private void ControlCounter()
    {
        if (counter <= 0)
        {
            controller.Lose();
        }
    }
}
