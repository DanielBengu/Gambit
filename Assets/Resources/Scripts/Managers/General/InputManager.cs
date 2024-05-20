using UnityEngine;

public static class InputManager
{
    public static bool IsClickDown()
    {
        return Input.GetMouseButtonDown(0);
    }
    public static bool IsClickUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    public static bool IsExit()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}