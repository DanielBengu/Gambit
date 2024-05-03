using UnityEngine;

//We divide lines into separate section so we can control speed for various sections
public class DialogueSection
{
    public string text;
    public float speed;
    public GameObject character;

    bool completed;

    public DialogueSection(string text, float speed, GameObject character)
    {
        this.text = text;
        this.speed = speed;
        this.character = character;
        completed = false;
    }

    public void SetCompletion(bool completed)
    {
        this.completed = completed;
    }

    public bool IsCompleted()
    {
        return completed;
    }
}