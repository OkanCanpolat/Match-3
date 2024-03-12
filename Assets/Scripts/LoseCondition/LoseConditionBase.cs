using TMPro;
using Zenject;


public abstract class LoseConditionBase
{
    protected int counter;
    protected TMP_Text counterText;
    protected TMP_Text counterNameText;
    protected string counterName;
    protected LoseContionController controller;

    public LoseConditionBase(int counter, TMP_Text counterText, TMP_Text counterNameText)
    {
        this.counter = counter;
        this.counterText = counterText;
        this.counterNameText = counterNameText;
    }
    public abstract void Init(LoseContionController controller);
    public class Factory : PlaceholderFactory<int, TMP_Text, TMP_Text, LoseConditionBase> { }
}

public class CustomLoseConditionFactory : IFactory<int, TMP_Text, TMP_Text, LoseConditionBase>
{
    private LoseConditionType type;
    private DiContainer container;

    public CustomLoseConditionFactory(LoseConditionType type, DiContainer container)
    {
        this.type = type;
        this.container = container;
    }

    public LoseConditionBase Create(int param1, TMP_Text param2, TMP_Text param3)
    {
        switch (type)
        {
            case LoseConditionType.Move:
                return container.Instantiate<MoveLoseCondition>(new object[] {param1, param2, param3});
        }
        
        return container.Instantiate<TimeLoseCondition>(new object[] { param1, param2, param3 });
    }
}

