﻿using System.Collections.Generic;
using System;
using static Map;

[Serializable]
public class MapData
{
    public List<Map> Maps;
}

[Serializable]
public class EncounterData
{
    public int Id;
    public TypeOfEncounter Type;
}

[Serializable]
public class Map
{
    public int Id;
    public string Name;
    public List<EncounterData> EncounterList;
    public int NumberOfEncounters;

    //This lists represents encounters that are fixed (for example forcing a skeleton enemy encounter as a first encounter on a world)
    public List<CustomEncounter> CustomEncounters;

    public enum TypeOfEncounter
    {
        Combat,
        Event
    }
}

[Serializable]
public class CustomEncounter : EncounterData
{
    public int PositionOnMap;
}