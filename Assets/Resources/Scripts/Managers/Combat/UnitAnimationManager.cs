using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AnimationManager;
using static FightManager;

public class UnitAnimationManager : MonoBehaviour
{
    FightManager _fightManager;

    FightManager FightManager { get { return GetManager(); }  }

    public Character Character { get { return GetOwnCharacter(); } }

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
            dictionaryCallback[animation]();
            dictionaryCallback.Remove(animation);
        }
    }

    public void PerformedAttack()
    {
        Character enemy = Character == Character.Player ? Character.Enemy : Character.Player;

        SpriteAnimation animation = FightManager.GetDamageAnimation(enemy);
        FightManager.MakeUnitPerformAnimation(enemy, animation);
    }

    public Character GetOwnCharacter()
    {
        return name.Equals("Player") ? Character.Player : Character.Enemy;
    }

    public FightManager GetManager()
    {
        if (_fightManager != null)
            return _fightManager;

        _fightManager = GameObject.Find("GameManager").GetComponent<GameManager>().FightManager;

        return _fightManager;
    }
}
