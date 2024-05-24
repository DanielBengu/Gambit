using static AnimationManager;
using static CardsManager;
using UnityEngine;
using static FightManager;
using System.Collections.Generic;
using System.Linq;
using System;
using static GameUIManager;
using System.Collections;

public class TurnManager
{
    public readonly FightManager fightManager;
    readonly GameUIManager gameUIManager;

    FightUnit Player { get; }
    GameObject PlayerObj { get { return fightManager.playerObj; } }
    FightUnit Enemy { get; }
    GameObject EnemyObj { get { return fightManager.enemyObj; } }

    public TurnStatus CurrentTurn { get; set; }
    public int TurnCount { get; set; } = 0;

    public Queue<AttackStruct> attacks = new();

    public TurnManager(FightUnit player, FightUnit enemy, FightManager manager, GameUIManager gameUIManager)
    {
        Player = player;
        Enemy = enemy;
        fightManager = manager;

        CurrentTurn = TurnStatus.IntermediaryEffects;
        this.gameUIManager = gameUIManager;
    }

    public void ResetTurn()
    {
        ResetUnitTurn(Player);
        ResetUnitTurn(Enemy);

        CurrentTurn = TurnStatus.PlayerTurn;
        TurnCount++;

        fightManager.DrawTurnHand();

        gameUIManager.UpdateUI(Enemy);
        gameUIManager.UpdateUI(Player);

        string damagePrevision = fightManager.GetDamagePrevision(Player, Enemy, out PrevisionEnum prev);
        gameUIManager.UpdatePrevision(prev, damagePrevision);

        gameUIManager.SetStandButtonInteractable(true);
    }

    void ResetUnitTurn(FightUnit unit)
    {
        unit.TickModifiers();
        unit.currentScore = 0;
        unit.status = CharacterStatus.Playing;

        unit.Class.ResetTurn();

        unit.FightCurrentHand.Clear();
    }

    public void HandleEndTurn()
    {
        int pointDifference = Mathf.Abs(Player.currentScore - Enemy.currentScore);

        if (pointDifference != 0)
        {
            FightUnit attacker = Player.currentScore > Enemy.currentScore ? Player : Enemy;
            FightUnit defender = Player.currentScore > Enemy.currentScore ? Enemy : Player;

            int damage = CalculateDamage(attacker, defender, pointDifference, false);

            SetAttacks(attacker, defender, damage, true);

            GameObject obj = attacker.Character == Character.Player ? PlayerObj : EnemyObj;

            string animation = GetAttackAnimation(attacker, obj);

            PlayCustomAnimation(obj, animation, DealDamage);
        }

        ResetTurn();
    }

    int CalculateDamage(FightUnit attacker, FightUnit defender, int baseDamage, bool isPiercing)
    {
        int attackerDamage = attacker.ApplyDamageModifiers(baseDamage);
        int finalDamageAmount = defender.ApplyDefenceModifiers(attackerDamage, isPiercing);

        return finalDamageAmount;
    }

    public void SetAttacks(FightUnit attacker, FightUnit defender, int damage, bool isEndTurn)
    {
        for (int i = 0; i < attacker.Attacks; i++)
            attacks.Enqueue(new(attacker, defender, damage, DealDamage));

        if(isEndTurn)
            AddBonusAttacks(attacker, defender);
    }

    void AddBonusAttacks(FightUnit attacker, FightUnit defender)
    {
        switch (attacker.Class.Class)
        {
            case Classes.Basic:
            case Classes.Warrior:
            case Classes.Wizard:
            case Classes.Monk:
            case Classes.Ranger:
            case Classes.Crystal:
            case Classes.Trickster:
            default:
                break;
            case Classes.Rogue:
                Rogue rogueClass = attacker.Class as Rogue;
                foreach (var attack in rogueClass.GetBonusAttacks(attacker, defender, DealDamage))
                {
                    attacks.Enqueue(attack);
                }
                break;
        }
    }
    public void DealDamage()
    {
        if (attacks.Count == 0)
            return;

        fightManager.gameManager.ShakeGround();

        AttackStruct attack = attacks.Dequeue();

        FightUnit unitTakingDamage = attack.defender;

        if(attack.damageAmount > 0)
            unitTakingDamage.FightHP -= attack.damageAmount;

        GameObject defender = attack.defender.Character == Character.Player ? PlayerObj : EnemyObj;

        fightManager.HandleCharacterHPVariation(attack.defender, defender);

        if (attacks.Count > 0)
        {
            GameObject newAttacker = attacks.First().attacker.Character == Character.Player ? PlayerObj : EnemyObj;
            string animation = GetAttackAnimation(attack.attacker, newAttacker);
            PlayCustomAnimation(newAttacker, animation, attacks.First().callback);
        }
    }

    public void StartAttackEffect(string effectName)
    {
        if (attacks.Count == 0)
            return;

        GameObject obj = null;
        Transform defenderTransform = null;
        Transform attackerTransform = null;

        switch (effectName)
        {
            case "RangerCrit":
                obj = Resources.Load<GameObject>($"Prefabs/Characters/Ranger/Crit/Crit");
                defenderTransform = attacks.First().defender.Character == Character.Player ? PlayerObj.transform : EnemyObj.transform;
                attackerTransform = attacks.First().attacker.Character == Character.Player ? PlayerObj.transform : EnemyObj.transform;
                break;
        }

        InstantiateEffect(obj, defenderTransform, attackerTransform.GetComponent<UnitAnimationManager>().dictionaryCallback);
    }

    void InstantiateEffect(GameObject obj, Transform parent, Dictionary<string, Action> callbacks)
    {
        if (obj == null || parent == null || callbacks == null)
            return;

        GameObject attackInstance = UnityEngine.Object.Instantiate(obj, parent.position, parent.rotation, parent.transform.parent);

        Animator anim = attackInstance.GetComponent<Animator>();
        anim.Play("Attack");

        var script = attackInstance.GetComponent<UnitAnimationManager>();
        script.dictionaryCallback = callbacks;
    }

    public string GetAttackAnimation(FightUnit attacker, GameObject attackerObj)
    {
        return attacker.Class.GetAttackAnimation(attacker, attacks, attackerObj);
    }
}