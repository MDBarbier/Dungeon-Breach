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
    private List<(Character ch, GameObject cgo, GameObject sq, Character attacker)> targetedCharacters;
    private CombatLogHandler combatLogHandler;

    // Start is called before the first frame update
    void Start()
    {
        targetedCharacters = new List<(Character ch, GameObject cgo, GameObject sq, Character attacker)>();
        diceRoller = new DiceRoller();
        dungeonManager = FindObjectOfType<DungeonManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal List<(Character ch, GameObject cgo, GameObject sq, Character attacker)> GetTargetedCharacters() => targetedCharacters;

    internal (bool, int) PerformAttack(Character attacker, Character target)
    {
        //Calculate if attack hits
        var d20 = diceRoller.RollDie(20);
        var attackRollModified = d20 + attacker.DEX;
        var attackHit = attackRollModified > target.AC ? true : false;
        var damageDealt = 0;

        if (attackHit)
        {
            damageDealt = attacker.STR + diceRoller.RollDie(6);
            combatLogHandler.CombatLog($"{attacker.Name} attacks {target.Name} and the attack is hits for {damageDealt} damage!");
            characterManager.ApplyDamage(target, damageDealt);
        }
        else
        {
            combatLogHandler.CombatLog($"{attacker.Name} attacks {target.Name} and the attack misses!");
        }

        return (attackHit, damageDealt);
    }

    internal (bool, int) PerformHealing(Character healer, Character target)
    {
        //Calculate if attack hits
        var d12 = diceRoller.RollDie(12);
        bool spellWorked = true;

        if (d12 < 3)
        {
            spellWorked = false;
            combatLogHandler.CombatLog($"{healer.Name} tries to heal {target.Name} but the spell fizzles out");
        }

        var amountHealed = d12 + healer.INT;

        combatLogHandler.CombatLog($"{healer.Name} tries to heal {target.Name} and heals {amountHealed} of hitpoints");
        {
            characterManager.ApplyDamage(target, -amountHealed);
        }        

        return (spellWorked, amountHealed);
    }

    internal Dictionary<(int, int), GameObject> GetTargetsForAttack(Character attacker, GameObject gameObject, int range)
    {
        ClearTargets();
        var currentX = gameObject.transform.position.x;
        var currentZ = gameObject.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid();
        var furnitureArray = dungeonManager.GetFurnitureArray();

        Dictionary<(int, int), GameObject> possibleAttacks = new Dictionary<(int, int), GameObject>();
        List<KeyValuePair<(int, int), GameObject>> listToCheck;

        if (range == 1)
        {
            listToCheck = GetNeighbours(currentX, currentZ, dungeonGrid, false);
        }
        else
        {
            listToCheck = GetColumns(currentX, currentZ, dungeonGrid, range);
        }

        foreach (var squareToCheck in listToCheck)
        {
            if (squareToCheck.Value == null)
            {
                continue;
            }

            var result = CheckSquareForPossibleAttack(squareToCheck);

            if (result)
            {
                //Get the character gameobject in the square to attack
                var characterToAttack = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);
                if (characterToAttack.Item2 != null)
                {
                    //Is the potential target on the same side?
                    if (characterToAttack.Item2.PlayerControlled != attacker.PlayerControlled)
                    {
                        targetedCharacters.Add((characterToAttack.Item2, characterToAttack.Item1, squareToCheck.Value, attacker));
                        possibleAttacks.Add((squareToCheck.Key.Item1, squareToCheck.Key.Item2), characterToAttack.Item1);
                    }
                }
            }
        }

        return possibleAttacks;
    }

    internal Dictionary<(int, int), GameObject> GetTargetsForAttack(Character attacker, GameObject gameObject, int range, bool targetFriendly, bool targetEnemy)
    {
        ClearTargets();
        var currentX = gameObject.transform.position.x;
        var currentZ = gameObject.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid();
        var furnitureArray = dungeonManager.GetFurnitureArray();

        Dictionary<(int, int), GameObject> possibleAttacks = new Dictionary<(int, int), GameObject>();
        List<KeyValuePair<(int, int), GameObject>> listToCheck;

        if (range == 1)
        {
            listToCheck = GetNeighbours(currentX, currentZ, dungeonGrid, false);
        }
        else
        {
            listToCheck = GetColumns(currentX, currentZ, dungeonGrid, range);
        }

        foreach (var squareToCheck in listToCheck)
        {
            if (squareToCheck.Value == null)
            {
                continue;
            }

            var result = CheckSquareForPossibleAttack(squareToCheck);

            if (result)
            {
                //Get the character gameobject in the square to attack
                var characterToAttack = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);
                if (characterToAttack.Item2 != null)
                {
                    if (targetEnemy && !targetFriendly)
                    {
                        //Is the potential target on the same side?
                        if (characterToAttack.Item2.PlayerControlled != attacker.PlayerControlled)
                        {
                            targetedCharacters.Add((characterToAttack.Item2, characterToAttack.Item1, squareToCheck.Value, attacker));
                            possibleAttacks.Add((squareToCheck.Key.Item1, squareToCheck.Key.Item2), characterToAttack.Item1);
                        }
                    }
                    else if (targetEnemy && targetFriendly)
                    {
                        targetedCharacters.Add((characterToAttack.Item2, characterToAttack.Item1, squareToCheck.Value, attacker));
                        possibleAttacks.Add((squareToCheck.Key.Item1, squareToCheck.Key.Item2), characterToAttack.Item1);
                    }
                    else if (targetFriendly && !targetEnemy)
                    {
                        //Is the potential target on the same side?
                        if (characterToAttack.Item2.PlayerControlled == attacker.PlayerControlled)
                        {
                            targetedCharacters.Add((characterToAttack.Item2, characterToAttack.Item1, squareToCheck.Value, attacker));
                            possibleAttacks.Add((squareToCheck.Key.Item1, squareToCheck.Key.Item2), characterToAttack.Item1);
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected combination of targetFriendly & targetEnemy");
                    }
                }
            }
        }

        return possibleAttacks;
    }

    private static List<KeyValuePair<(int, int), GameObject>> GetNeighbours(float currentX, float currentZ, Dictionary<(int, int), GameObject> dungeonGrid, bool eightWay)
    {

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        List<KeyValuePair<(int, int), GameObject>> listToCheck = new List<KeyValuePair<(int, int), GameObject>>();

        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ + 1)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ - 1)).FirstOrDefault());

        if (eightWay)
        {
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ + 1)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ - 1)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ + 1)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ - 1)).FirstOrDefault()); 
        }

        return listToCheck;
    }

    private static List<KeyValuePair<(int, int), GameObject>> GetColumns(float currentX, float currentZ, Dictionary<(int, int), GameObject> dungeonGrid, int range)
    {

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        List<KeyValuePair<(int, int), GameObject>> listToCheck = new List<KeyValuePair<(int, int), GameObject>>();

        for (int i = 1; i <= range; i++)
        {
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + i, currentZ)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ + i)).FirstOrDefault());
        }

        for (int i = 1; i <= range; i++)
        {
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - i, currentZ)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ - i)).FirstOrDefault());
        }

        return listToCheck;
    }

    internal void ClearTargets()
    {
        targetedCharacters.Clear();
        targetedCharacters = new List<(Character ch, GameObject cgo, GameObject sq, Character attacker)>();
    }

    /// <summary>
    /// Selects a target for the AI
    /// </summary>
    /// <param name="attacker">The attacker's character instance</param>
    /// <param name="gameObject">The attacker's game object</param>
    /// <returns></returns>
    internal ((int, int), GameObject) SelectTarget(Character attacker, GameObject gameObject)
    {
        var currentX = gameObject.transform.position.x;
        var currentZ = gameObject.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid();
        var furnitureArray = dungeonManager.GetFurnitureArray();
        Dictionary<(int, int), GameObject> possibleAttacks = new Dictionary<(int, int), GameObject>();

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        List<KeyValuePair<(int, int), GameObject>> listToCheck = new List<KeyValuePair<(int, int), GameObject>>();

        //todo :BUG:low: fix this loop so that it correctly looks at where character is and works out concentrically
        for (int i = 0; i < dungeonGrid.Count; i++)
        {
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + i, currentZ)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - i, currentZ)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ + i)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ - i)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + i, currentZ + i)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + i, currentZ - i)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - i, currentZ + i)).FirstOrDefault());
            listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - i, currentZ - i)).FirstOrDefault());

            foreach (var squareToCheck in listToCheck)
            {
                if (squareToCheck.Value == null)
                {
                    continue;
                }

                var result = CheckSquareForPossibleAttack(squareToCheck);

                if (result)
                {
                    //Get the character gameobject in the square to attack
                    var characterToAttack = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);
                    if (characterToAttack.Item2 != null)
                    {
                        //Is the potential target a character on the same side?
                        if (characterToAttack.Item2.PlayerControlled != attacker.PlayerControlled)
                        {
                            return ((squareToCheck.Key.Item1, squareToCheck.Key.Item1), characterToAttack.Item1);
                        }
                    }
                }
            }
        }

        throw new Exception($"{attacker.Name} scratches their head and looks confused...");
    }

    private bool CheckSquareForPossibleAttack(KeyValuePair<(int, int), GameObject> squareToCheck)
    {
        if (squareToCheck.Value != null)
        {
            //see if the square is occupied: (check character lists, furniture list)
            var characterInPosition = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);

            //if not, add them to viable attack list
            if (characterInPosition.Item1 != null)
            {
                return true;
            }
        }

        return false;
    }
}
