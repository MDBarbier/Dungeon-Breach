using System;

namespace Assets.Scripts.NonMonoBehaviour
{
    class DiceRoller
    {
        private Random random;

        public DiceRoller()
        {
            random = new Random();
        }

        public int RollDie(int sides) => random.Next(1, sides);        

        public int[] RollDie(int sides, int number)
        {
            var array = new int[number];
            for (int i = 0; i < number; i++)
            {
                array[i] = random.Next(1, sides);
            }
            return array;
        }
    }
}
