
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Classes
{
    public class Character
    {
        public string Name { get; set; }
        public int STR { get; set; }
        public int CON { get; set; }
        public int INT { get; set; }
        public int DEX { get; set; }
        public int CHA { get; set; }
        public int HP { get; set; }
        public int AC { get; set; }
        public bool PlayerControlled { get; set; }
        public string Backstory { get; set; }
        public int Level { get; set; }
        public int MA { get; set; }
        public object MAXHP { get; internal set; }
        public Races Race { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Selected unit details\n");
            sb.Append($"Name: {this.Name}\n");
            sb.Append($"HP: {this.HP}/{this.MAXHP}\n");
            sb.Append($"AC: {this.AC}\n");
            sb.Append($"STR: {this.STR}\n");
            sb.Append($"DEX: {this.DEX}\n");
            return sb.ToString();
        }

        public void SetRandomName(Random random, List<Character> characterList)
        {
            int enumMemberCount;
            int randNumber;

            switch (this.Race)
            {
                case Races.Human:
                    enumMemberCount = Enum.GetNames(typeof(HumanNames)).Length;

                    for (int i = 0; i < enumMemberCount * 3; i++)
                    {
                        randNumber = random.Next(enumMemberCount);
                        var humanName = (HumanNames)randNumber;
                        var match = characterList.Where(a => a.Name == humanName.ToString()).FirstOrDefault();
                        if (match == null)
                        {
                            this.Name = humanName.ToString();
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(this.Name))
                    {
                        throw new Exception("Did not find an unused name successfully");
                    }

                    break;
                case Races.Orc:
                    enumMemberCount = Enum.GetNames(typeof(OrcNames)).Length;

                    for (int i = 0; i < enumMemberCount * 3; i++)
                    {
                        randNumber = random.Next(enumMemberCount);
                        var orcName = (OrcNames)randNumber;
                        var match = characterList.Where(a => a.Name == orcName.ToString()).FirstOrDefault();
                        if (match == null)
                        {
                            this.Name = orcName.ToString();
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(this.Name))
                    {
                        throw new Exception("Did not find an unused name successfully");
                    }

                    break;
                case Races.Elf:
                case Races.Dwarf:
                case Races.Goblin:
                case Races.Halfling:
                case Races.Troll:
                default:
                    throw new Exception("Not implemented");
            }
        }
    }

    public class Fighter : Character
    {
        public Fighter()
        {
            this.STR = 14;
            this.DEX = 14;
            this.CON = 16;
            this.CHA = 10;
            this.HP = this.CON * 8;
            this.MAXHP = this.HP;
            this.INT = 10;
            this.Level = 1;
            this.MA = this.DEX / 4 + 1;
            this.AC = this.DEX / 4 + 10;
        }
    }
    public class Bandit : Character
    {
        public Bandit()
        {
            this.STR = 11;
            this.DEX = 14;
            this.CON = 9;
            this.CHA = 10;
            this.HP = this.CON * 8;
            this.MAXHP = this.HP;
            this.INT = 8;
            this.Level = 1;
            this.MA = this.DEX / 4 + 1;
            this.AC = this.DEX / 4 + 10;
        }
    }
}
