using static FightUnit;

namespace Assets.Resources.Scripts.Fight
{
    public static class ActionCardArchive
    {
        public static void ApplyEffect(int cardId, FightUnit unit)
        {
            switch (cardId)
            {
                case 0:
                    ApplyModifier(unit, Stats.Attacks, 1, 1);
                    break;
                case 1: ApplyModifier(unit, Stats.Damage, 1, 2); 
                    break;
                case 2:
                    ApplyModifier(unit, Stats.Armor, 1, 2);
                    break;
                case 3:
                    AddScore(unit, 2);
                    break;
                case 4:
                    AddScore(unit, 6);
                    break;
            }
        }

        static void ApplyModifier(FightUnit unit, Stats stat, int turnCount, int modifier)
        {
            unit.CurrentModifiers.Add(new(stat, turnCount, modifier));
        }
        static void AddScore(FightUnit unit, int modifier)
        {
            unit.currentScore += modifier;
        }
    }
}
