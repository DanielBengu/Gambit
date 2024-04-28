using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData
{
    public int Id;
    public string[] Dialogue;
}

[Serializable]
public class DialogueContainer
{
    public List<DialogueData> Dialogues;
}