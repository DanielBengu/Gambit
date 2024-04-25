using Assets.Resources.Scripts.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class EnemyManager : MonoBehaviour
{
    public FightManager fightManager;
    public EnemyCurrent Enemy { get; set; }
    public TurnStatus TurnStatus { get { return fightManager.CurrentTurn; } set { fightManager.CurrentTurn = value; } }

    public void PlayEnemyTurn()
    {
        fightManager.PlayUnitCard(Character.Enemy);
    }

    public void HandleEnemyTurn()
    {
        if (Enemy.Status != CharacterStatus.Playing)
            return;

        //Player is bust or is standing, enemy keeps playing the turn until end
        PlayEnemyTurn();

        if (Enemy.Status == CharacterStatus.Playing && Enemy.CurrentScore >= Enemy.Threshold)
            Enemy.Status = CharacterStatus.Standing;

        if (Enemy.Status != CharacterStatus.Playing)
            TurnStatus = TurnStatus.PlayerTurn;
    }

    public class EnemyCurrent
    {
        public string Name { get; set; }

        public int Armor { get; set; }
        public int HP { get; set; }

        public List<GameCard> CurrentDeck { get; set; }
        public List<GameCard> BaseDeck { get; set; }

        public int CurrentScore { get; set; }
        public int MaxScore { get; set; }
        public int Threshold { get; set; }
        public CharacterStatus Status { get; set; }

        public EnemyCurrent(EnemyData enemy)
        {
            Name = enemy.Name;

            Armor = enemy.Armor;
            HP = enemy.HP;

            BaseDeck = enemy.BaseDecklist;
            CurrentDeck = enemy.BaseDecklist;

            CurrentScore = 0;
            MaxScore = enemy.BaseMaxScore;
            Threshold = enemy.BaseStandThreshold;

            Status = CharacterStatus.Playing;
        }
    }
}