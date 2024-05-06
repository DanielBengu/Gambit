using System;
using System.Collections.Generic;
using UnityEngine;
public class UnitAnimationManager : MonoBehaviour
{
    public Dictionary<string, Action> dictionaryCallback = new();


    public void SaveAnimationCallback(string animationToPlay, Action callback)
    {
        //If another animation is already playing on the character we call its callback and then clear to make space to the new anim
        StartCallback(animationToPlay);

        dictionaryCallback.Add(animationToPlay, callback);
    }

    public void StartCallback(string animation)
    {
        if (dictionaryCallback.ContainsKey(animation))
        {
            Action callback = dictionaryCallback[animation];
            dictionaryCallback.Remove(animation);
            callback();
        }
    }

    public void StartEffect(string effect)
    {
        FightManager fm = GameObject.Find("GameManager").GetComponent<GameManager>().FightManager;
        fm.StartAttackEffect(effect);
    }

    public void Terminate()
    {
        Destroy(gameObject);
    }
}
