
using Assets.Scripts.Enums;

namespace Assets.Scripts.Classes
{
    public class RangedCharacter : Character
    {
        public RangedCharacter()
        {
            this.STR = 10;
            this.DEX = 18;
            this.CON = 4;
            this.CHA = 12;
            this.HP = this.CON * 8;
            this.MAXHP = this.HP;
            this.INT = 10;
            this.Level = 1;
            this.MA = this.DEX / 4 + 1;
            this.AC = this.DEX / 4 + 7;
            this.Range = 100;
            this.DamageType = DamageTypes.Physical;
        }
    }
}
