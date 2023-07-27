namespace Toolbox.Patterns.Strategy
{
    public class Fire_Sword : WeaponBase
    {
        public Fire_Sword()
        {
            damage = 40;
            damageType = new FireDamage();
        }
    }
}
