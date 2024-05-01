using System;
using System.Collections.Generic;
using static CardsManager;

namespace Assets.Resources.Scripts.Fight
{
    [Serializable]
    public class EnemyData
    {
        public int Id;

        public Classes UnitClass;

        public string Name;

        public bool IsCustomDecklist;
        public List<GameCard> BaseDecklist;

        public List<Reward> Rewards;

        public int HP;
        public int Armor;

        public int BaseMaxScore;

        public int BaseStandThreshold;
    }

    [Serializable]
    public class EnemyList
    {
        public List<EnemyData> Enemies;
    }
}
