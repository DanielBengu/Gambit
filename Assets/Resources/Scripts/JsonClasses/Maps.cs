using System.Collections.Generic;
using System;

[Serializable]
public class MapData
{
    public List<Map> Maps;
}

[Serializable]
public class Map
{
    public int Id;
    public string Name;
    public List<int> EnemiesListId;
}