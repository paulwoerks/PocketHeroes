namespace Toolbox.Patterns.Strategy
{
    public class Fire_Dagger : WeaponBase
    {
        public Fire_Dagger()
        {
            damage = 10;
            damageType = new FireDamage();
        }
    }
}
