using Assets.Resources.Scripts.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class EnemyManager : MonoBehaviour
{
    public FightManager fightManager;
    public FightUnit Enemy { get; set; }
    public TurnStatus TurnStatus { get { return fightManager.CurrentTurn; } set { fightManager.CurrentTurn = value; } }

    public void PlayEnemyTurn()
    {
        fightManager.PlayUnitCard(Character.Enemy);
    }

    public void HandleEnemyTurn()
    {
        if (Enemy.status != CharacterStatus.Playing)
            return;

        //Player is bust or is standing, enemy keeps playing the turn until end
        PlayEnemyTurn();

        if (Enemy.status == CharacterStatus.Playing && Enemy.currentScore >= Enemy.Threshold)
            Enemy.status = CharacterStatus.Standing;

        if (Enemy.status != CharacterStatus.Playing)
            TurnStatus = TurnStatus.PlayerTurn;
    }

    public class EnemyCurrent
    {
        public string Name { get; set; }

        public int Armor { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public List<GameCard> CurrentDeck { get; set; }
        public List<GameCard> BaseDeck { get; set; }

        public int currentScore;
        public int maxScore;
        public int Threshold { get; set; }

        public CharacterStatus status;

        public EnemyCurrent(EnemyData enemy)
        {
            Name = enemy.Name;

            Armor = enemy.Armor;
            MaxHP = enemy.HP;
            CurrentHP = enemy.HP;

            BaseDeck = GameManager.CopyDeck(enemy.BaseDecklist);
            CurrentDeck = GameManager.CopyDeck(enemy.BaseDecklist);

            currentScore = 0;
            maxScore = enemy.BaseMaxScore;
            Threshold = enemy.BaseStandThreshold;

            status = CharacterStatus.Playing;
        }
    }
}