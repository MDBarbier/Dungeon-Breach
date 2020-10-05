using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Nonmonobehaviour
{
    class JaggedArrayMethods
    {
        internal static GameObject[][] InitializeJaggedArray(int xaxis, int zaxis, GameObject[][] jaggedArray)
        {
            for (int i = 0; i < xaxis; i++)
            {
                jaggedArray[i] = new GameObject[zaxis];
            }

            return jaggedArray;
        }


        internal static string GetJaggedArrayOutputString(GameObject[][] gameObjectsArray)
        {
            var outerLength = gameObjectsArray.Length;
            string output = "";

            for (int i = 0; i < outerLength; i++)
            {
                var innerLength = gameObjectsArray[i].Length;

                for (int j = 0; j < innerLength; j++)
                {
                    if (gameObjectsArray[i][j] != null)
                    {
                        output += $"X: {i}, Z: {j}, Name: {gameObjectsArray[i][j].name}\n";
                    }
                }
            }

            return output;
        }
    }
}
