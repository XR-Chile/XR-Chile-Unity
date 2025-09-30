using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TimeEventMonitor : MonoBehaviour
{
    private float _sessionTimer = 0f;
    private List<EventRecord> _eventRecords = new List<EventRecord>();

    [System.Serializable]
    public class EventRecordListWrapper
    {
        public List<EventRecord> Events;
    }

    void OnEnable()
    {
        FireObject.Extinguishing += () => RecordEvent("Extinguish");
        FireParticle.ParticleCollision += () => RecordEvent("Waste");

        _sessionTimer = 0;
        _eventRecords?.Clear();
    }

    void OnDisable()
    {
        FireObject.Extinguishing -= () => RecordEvent("Extinguish");
        FireParticle.ParticleCollision -= () => RecordEvent("Waste");

        SaveEvents();
    }

    private void Update()
    {
        CountTime();
    }

    private void CountTime()
    {
        _sessionTimer += Time.deltaTime;
    }

    public void RecordEvent(string eventName)
    {
        _eventRecords.Add(new EventRecord(eventName, _sessionTimer));
        Debug.Log($"Evento registrado: {eventName} en {_sessionTimer:F2} segundos");
    }

    public List<(string EventName, float Timestamp)> GetEventTimeline()
    {
        var timeline = new List<(string, float)>();
        foreach (var record in _eventRecords)
        {
            timeline.Add((record.EventName, record.Timestamp));
        }
        return timeline;
    }

    private void SaveEvents()
    {
        EventRecordListWrapper wrapper = new EventRecordListWrapper { Events = _eventRecords };
        string path = Path.Combine(Application.persistentDataPath, "EventRecords.json");
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(path, json);
        Debug.Log($"Eventos guardados en {path}");
    }
}
