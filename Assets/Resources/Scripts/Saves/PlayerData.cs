[System.Serializable]
public class PlayerData
{
    public CurrentRun CurrentRun;
}

[System.Serializable]
public class CurrentRun
{
    public bool IsOngoing;
    public string Class;
    public int CurrentFloor;
}
