using Assets.Scripts.Classes;
using Assets.Scripts.NonMonoBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private DiceRoller diceRoller;
    private DungeonManager dungeonManager;
    private CharacterManager characterManager;

    // Start is called before the first frame update
    void Start()
    {
        diceRoller = new DiceRoller();
        dungeonManager = FindObjectOfType<DungeonManager>();
        characterManager = FindObjectOfType<CharacterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal (bool, int) AttackCharacter(Character attacker, Character target)
    {
        //Calculate if attack hits
        var d20 = diceRoller.RollDie(20);
        var attackRollModified = d20 + attacker.DEX;
        var attackHit = attackRollModified > target.AC ? true: false;
        var damageDealt = 0;

        if (attackHit)
        {
            damageDealt = attacker.STR + diceRoller.RollDie(6);
        }

        return (attackHit, damageDealt);
    }

    internal Dictionary<(int,int), GameObject> GetTargetsForAttack(Character attacker, GameObject gameObject)
    {
        int range = 1;
        var currentX = gameObject.transform.position.x;
        var currentZ = gameObject.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid(); //todo refactor into getter on dungeon manager
        var furnitureArray = dungeonManager.GetFurnitureArray();
        Dictionary<(int, int), GameObject> possibleAttacks = new Dictionary<(int, int), GameObject>();

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        var upperSquare = dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ)).FirstOrDefault();
        var lowerSquare = dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ)).FirstOrDefault();
        var leftSquare = dungeonGrid.Where(a => a.Key == (currentX, currentZ + 1)).FirstOrDefault();
        var rightSquare = dungeonGrid.Where(a => a.Key == (currentX, currentZ - 1)).FirstOrDefault();
        var upperLeftSquare = dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ +1)).FirstOrDefault();
        var upperRightSquare = dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ - 1)).FirstOrDefault();
        var lowerLeftSquare = dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ + 1)).FirstOrDefault();
        var lowerRightSquare = dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ - 1)).FirstOrDefault();

        var result = CheckSquareForPossibleAttack(possibleAttacks, upperSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, lowerSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, leftSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, rightSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, upperRightSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, upperLeftSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, lowerLeftSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleAttack(possibleAttacks, lowerRightSquare);

        if (result.Item2 != null)
        {
            possibleAttacks.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        return possibleAttacks;
    }

    private ValueTuple<(int, int), GameObject> CheckSquareForPossibleAttack(Dictionary<(int, int), GameObject> possibleMoves, KeyValuePair<(int, int), GameObject> squareToCheck)
    {
        if (squareToCheck.Value != null)
        {
            //see if the square is occupied: (check character lists, furniture list)
            var characterInPosition = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);            

            //if not, add them to move list
            if (characterInPosition.Item1 != null)
            {
                return ((squareToCheck.Key.Item1, squareToCheck.Key.Item2), squareToCheck.Value);
            }
        }

        return ((0, 0), null);
    }
}
