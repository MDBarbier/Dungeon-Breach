
using Assets.Scripts.Enums;

namespace Assets.Scripts.Classes
{
    public class MagicCharacter : Character
    {
        public MagicCharacter()
        {
            this.STR = 6;
            this.DEX = 10;
            this.CON = 4;
            this.CHA = 16;
            this.HP = this.CON * 8;
            this.MAXHP = this.HP;
            this.INT = 16;
            this.Level = 1;
            this.MA = this.DEX / 4 + 1;
            this.AC = this.DEX / 4 + 5;
            this.Range = 100;
        }

        public void SetDamageType(DamageTypes damageType)
        {
            this.DamageType = damageType;
        }
    }
}
