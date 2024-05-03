using UnityEngine;

public static class InputManager
{
    public static bool IsClick()
    {
        return Input.GetMouseButtonDown(0);
    }
}