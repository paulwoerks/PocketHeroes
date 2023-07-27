namespace Toolbox.Patterns.Strategy
{
    // RES: https://www.youtube.com/watch?v=yjZsAl13trk
    public class WeaponBase
    {
        public int damage = 0;
        public IDoDamage damageType; // For Fire/Ice Sword just make a list of IDoDamage and execute everyone at TryDoAttack
        public void TryDoAttack() => damageType.DoDamage(damage);
        public void SetDamageType(IDoDamage damageType) => this.damageType = damageType;
    }
}