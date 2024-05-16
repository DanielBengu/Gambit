using System;
using UnityEngine;
using static CardsManager;

public class UnlockableCardScript : MonoBehaviour
{
    Action callback;
    PlayerPrefsManager.PlayerPrefsEnum unlock;

    public void LoadScript(Action callback, PlayerPrefsManager.PlayerPrefsEnum unlock)
    {
        this.callback = callback;
        this.unlock = unlock;
    }

    private void OnMouseDown()
    {
        PlayerPrefsManager.SetPref(unlock, 1);
        callback();
    }
}
