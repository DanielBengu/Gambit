using System;
using System.Collections.Generic;

namespace Assets.Resources.Scripts.Fight
{
    [Serializable]
    public class Enemy
    {
        public string Name;
        public List<GameCard> BaseDecklist;
        public List<Reward> Rewards;

        public int HP;
        public int Armor;

        public int BaseMaxScore;

        public int BaseStandThreshold;
    }
}
