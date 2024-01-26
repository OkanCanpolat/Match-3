using System;
using System.Collections.Generic;
public enum EventBusType
{
    Blue, Orange, Red, Purple, Green, Yellow, 
    BlueRowBomb, OrangeRowBomb, RedRowBomb, PurpleRowBomb, GreenRowBomb, YellowRowBomb,
    BlueColumnBomb, OrangeColumnBomb, RedColumnBomb, PurpleColumnBomb, GreenColumnBomb, YellowColumnBomb,
    BlueWrapped, OrangeWrapped, RedWrapped, PurpleWrapped, GreenWrapped, YellowWrapped,
    ColorBomb, BreakableL1, BreakableL2
}

public class EventBus
{
    private static readonly Dictionary<EventBusType, Action<EventBusType>> events = new Dictionary<EventBusType, Action<EventBusType>>();

    public static void Subscribe(EventBusType type, Action<EventBusType> dictionarydelegate)
    {
        if (!events.ContainsKey(type))
        {
            events.Add(type, dictionarydelegate);
        }
    }
    public static void Unsubscribe(EventBusType type, Action<EventBusType> dictionarydelegate)
    {
        if (events.ContainsKey(type))
        {
            events.Remove(type);
        }
    }

    public static void Publish(EventBusType type)
    {
        Action<EventBusType> thisEvent;

        if (events.TryGetValue(type, out thisEvent))
        {
            thisEvent?.Invoke(type);
        }
    }
    public static void ClearEvents()
    {
        events.Clear();
    }

}
