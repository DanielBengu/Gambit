using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class UnitAnimationManager : MonoBehaviour
{
    public void UnitAttacked()
    {
        //should be improved but putting it in start breaks because it's loaded after, should add manual valorization from gamemanager
        FightManager fightManager = GameObject.Find("GameManager").GetComponent<GameManager>().fightManager;

        Character enemy = fightManager.GetCharacterFromGameObjectID(GetInstanceID()) == Character.Player ? Character.Enemy : Character.Player;
        
        fightManager.MakeUnitTakeDamage(enemy);
    }
}
