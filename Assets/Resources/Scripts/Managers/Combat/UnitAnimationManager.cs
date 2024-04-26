using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using static FightManager;

public class UnitAnimationManager : MonoBehaviour
{
    FightManager _fightManager;

    FightManager FightManager { get { return GetManager(); }  }

    public Character Character { get { return GetOwnCharacter(); } }

    public void PerformedAttack()
    {
        Character enemy = Character == Character.Player ? Character.Enemy : Character.Player;

        SpriteAnimation animation = FightManager.GetDamageAnimation(enemy);
        FightManager.MakeUnitPerformAnimation(enemy, animation);
    }

    public void Death()
    {
        FightManager.RemoveObjFromAnimationList(gameObject);
        FightManager.HandleUnitDeath(Character);
    }

    public Character GetOwnCharacter()
    {
        return FightManager.GetCharacterFromGameObjectID(gameObject.GetInstanceID()) == Character.Player ? Character.Player : Character.Enemy;
    }

    public FightManager GetManager()
    {
        if (_fightManager != null)
            return _fightManager;

        _fightManager = GameObject.Find("GameManager").GetComponent<GameManager>().fightManager;

        return _fightManager;
    }
}
