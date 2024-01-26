using TMPro;


public abstract class LoseCondition
{
    protected int counter;
    protected TMP_Text counterText;
    protected TMP_Text counterNameText;
    protected string counterName;
    protected LoseContionController controller;

    public LoseCondition(int counter, TMP_Text counterText, TMP_Text counterNameText)
    {
        this.counter = counter;
        this.counterText = counterText;
        this.counterNameText = counterNameText;
    }
    public abstract void Init(LoseContionController controller);
}
