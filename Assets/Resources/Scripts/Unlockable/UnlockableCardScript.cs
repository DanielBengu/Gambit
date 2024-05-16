using System;
using UnityEngine;
using static CardsManager;

public class UnlockableCardScript : MonoBehaviour
{
    Action callback;

    public void LoadScript(Action callback)
    {
        this.callback = callback;
    }

    private void OnMouseDown()
    {
        PlayerPrefsManager.SetPref(PlayerPrefsManager.PlayerPrefsEnum.BerserkUnlocked, 1);
        callback();
    }
}
