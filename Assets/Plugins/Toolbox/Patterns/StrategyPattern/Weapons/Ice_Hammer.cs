namespace Toolbox.Patterns.Strategy
{
    public class Ice_Hammer : WeaponBase
    {
        public Ice_Hammer()
        {
            damage = 20;
            damageType = new IceDamage();
        }
    }
}