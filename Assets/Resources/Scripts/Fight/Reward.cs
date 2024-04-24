namespace Assets.Resources.Scripts.Fight
{
    public class Reward
    {
        public TypeOfReward reward;
        public int rewardId;
        public int amount;
    }

    public enum TypeOfReward
    {
        Gold,
        Equipment
    }
}
