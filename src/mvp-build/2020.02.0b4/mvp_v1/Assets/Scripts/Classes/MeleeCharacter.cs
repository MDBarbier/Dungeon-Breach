
using Assets.Scripts.Enums;

namespace Assets.Scripts.Classes
{
    public class MeleeCharacter : Character
    {
        public MeleeCharacter()
        {
            this.STR = 14;
            this.DEX = 14;
            this.CON = 10;
            this.CHA = 10;
            this.HP = this.CON * 8;
            this.MAXHP = this.HP;
            this.INT = 10;
            this.Level = 1;
            this.MA = this.DEX / 4 + 1;
            this.AC = this.DEX / 4 + 10;
            this.Range = 1;
            this.DamageType = DamageTypes.Physical;
        }
    }
}
