using System;
using System.Collections.Generic;

namespace Assets.Resources.Scripts.Fight
{
    [Serializable]
    public class Enemy
    {
        public string Name { get; set; }
        public List<GameCard> BaseDecklist { get; set; }
        public List<Reward> Rewards { get; set; }
    }
}
