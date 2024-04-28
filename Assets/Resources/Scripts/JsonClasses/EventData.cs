using static Map;
using System.Collections.Generic;
using System;
using static EventData;
using JetBrains.Annotations;

[Serializable]
public class EventList
{
    public List<EventData> Events;
}

[Serializable]
public class EventData
{
    public int Id;
    public string Description;
    public List<EventOption> EventOptions; 

    public enum EncounterEvents
    {
        RaiseArmor,
        RaiseMaxHP
    }
}

[Serializable]
public class EventOption
{
    public EncounterEvents EncounterEvent;
    public string Description;
}