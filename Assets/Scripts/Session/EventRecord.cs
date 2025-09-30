public class EventRecord
{
    public string EventName;
    public float Timestamp;

    public EventRecord(string eventName, float timestamp)
    {
        EventName = eventName;
        Timestamp = timestamp;
    }
}