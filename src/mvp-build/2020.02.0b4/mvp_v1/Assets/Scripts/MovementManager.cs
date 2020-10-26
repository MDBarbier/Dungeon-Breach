using Assets.Scripts.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private CharacterManager characterManager;
    private SelectionManager selectionManager;
    private CombatManager combatManager;
    private GameObject lastSelectedGameObject;
    private TurnManager turnManager;
    private Pathfinder pathfinder;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] Material dungetonTileSelected;
    [SerializeField] Material dungetonTileSelectedAttack;
    [SerializeField] Material dungetonTileSelectedHeal;
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
        dungeonManager = FindObjectOfType<DungeonManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        turnManager = FindObjectOfType<TurnManager>();
        combatManager = FindObjectOfType<CombatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //todo refactor this update logic into Player character manager
        if (selectionManager.selectedCharacter.Item1 != null)
        {
            if (lastSelectedGameObject == selectionManager.selectedCharacter.Item1)
            {
                return;
            }

            //assign the gameobject we just processed to the last game object flag
            if (debugLogging) print($"assign last selected game object for {selectionManager.selectedCharacter.Item1.name}");
            lastSelectedGameObject = selectionManager.selectedCharacter.Item1;

            selectionManager.ResetHighlightedTiles();

            if (debugLogging) print($"Getting possible moves for {selectionManager.selectedCharacter.Item1.name}");

            Dictionary<(int, int), GameObject> possibleAttacks = new Dictionary<(int, int), GameObject>();
            Dictionary<(int, int), GameObject> possibleHeals = new Dictionary<(int, int), GameObject>();

            if (selectionManager.selectedCharacter.Item2.PlayerControlled)
            {
                var possibleMoves = pathfinder.GetPlayerMoveArea(selectionManager.selectedCharacter.Item1.transform.localPosition);
                if (debugLogging) print($"highlighting possible moves for {selectionManager.selectedCharacter.Item1.name}");

                foreach (var move in possibleMoves)
                {
                    if (move.Value.Item2 > selectionManager.selectedCharacter.Item2.MA || 
                        characterManager.GetCharacterAtPosition((int)move.Key.x, (int)move.Key.z).Item1 != null)
                    {
                        continue;
                    }
                    
                    var tile = dungeonManager.GetFloorTileByLocation(move.Key.x, move.Key.z);
                    selectionManager.AddhighlightedObject((((int)move.Key.x, (int)move.Key.z), (tile, tile.GetComponent<MeshRenderer>().material)));
                    tile.GetComponent<MeshRenderer>().material = dungetonTileSelected;
                }

                if (selectionManager.selectedCharacter.Item2.DamageType == Assets.Scripts.Enums.DamageTypes.Physical)
                {
                    if (debugLogging) print($"Getting possible physical attacks for {selectionManager.selectedCharacter.Item1.name}");
                    possibleAttacks = combatManager.GetTargetsForAttack(selectionManager.selectedCharacter.Item2, selectionManager.selectedCharacter.Item1, selectionManager.selectedCharacter.Item2.Range);
                }
                else if (selectionManager.selectedCharacter.Item2.DamageType == Assets.Scripts.Enums.DamageTypes.Healing)
                {
                    if (debugLogging) print($"Getting possible heals for {selectionManager.selectedCharacter.Item1.name}");
                    possibleHeals = combatManager.GetTargetsForAttack(selectionManager.selectedCharacter.Item2, selectionManager.selectedCharacter.Item1, selectionManager.selectedCharacter.Item2.Range, true, false);
                }
                else if (selectionManager.selectedCharacter.Item2.DamageType == Assets.Scripts.Enums.DamageTypes.Magic)
                {
                    if (debugLogging) print($"Getting possible magic attacks for {selectionManager.selectedCharacter.Item1.name}");
                    possibleAttacks = combatManager.GetTargetsForAttack(selectionManager.selectedCharacter.Item2, selectionManager.selectedCharacter.Item1, selectionManager.selectedCharacter.Item2.Range, true, true);
                }

                foreach (var tile in possibleAttacks)
                {
                    if (debugLogging) print($"highlighting possible attacks for {selectionManager.selectedCharacter.Item1.name}");
                    selectionManager.AddhighlightedObject(((tile.Key.Item1, tile.Key.Item2), (tile.Value, tile.Value.GetComponent<MeshRenderer>().material)));
                    tile.Value.GetComponent<MeshRenderer>().material = dungetonTileSelectedAttack;
                }

                foreach (var tile in possibleHeals)
                {
                    if (debugLogging) print($"highlighting possible heals for {selectionManager.selectedCharacter.Item1.name}");
                    selectionManager.AddhighlightedObject(((tile.Key.Item1, tile.Key.Item2), (tile.Value, tile.Value.GetComponent<MeshRenderer>().material)));
                    tile.Value.GetComponent<MeshRenderer>().material = dungetonTileSelectedHeal;
                }
            }            
        }
        else
        {
            selectionManager.ResetHighlightedTiles();
            lastSelectedGameObject = null;
        }
    }

    internal void MoveCharacter(ValueTuple<GameObject, Character> theCharacter, ValueTuple<(int, int), GameObject> squareToMoveTo)
    {
        //Perform the physical move of the gameobject
        if (debugLogging) print($"Perform the physical move of the gameobject for {theCharacter.Item1.name}");
        theCharacter.Item1.transform.localPosition = new Vector3(squareToMoveTo.Item1.Item1, theCharacter.Item1.transform.position.y, squareToMoveTo.Item1.Item2);

        //Report the change back to the character controller
        if (debugLogging) print($"Report the change back to the character controller for {theCharacter.Item1.name}");
        characterManager.UpdateCharacterPosition(theCharacter);

        //Tell turn controller to update initiative for this character
        if (debugLogging) print($"Tell turn controller to update initiative for this character for {theCharacter.Item2.Name}");
        turnManager.UpdateInitiativeTracker(theCharacter.Item2);

        //Remove selections
        if (debugLogging) print($"Removing selections and highlighted tiles");
        
        selectionManager.RemoveSelections();
        selectionManager.ResetHighlightedTiles();
    }

    internal void MoveCharacter(ValueTuple<GameObject, Character> theCharacter, Vector3 squareToMoveTo)
    {
        //Perform the physical move of the gameobject
        if (debugLogging) print($"Perform the physical move of the gameobject for {theCharacter.Item1.name}");
        theCharacter.Item1.transform.localPosition = new Vector3(squareToMoveTo.x, theCharacter.Item1.transform.position.y, squareToMoveTo.z);

        //Report the change back to the character controller
        if (debugLogging) print($"Report the change back to the character controller for {theCharacter.Item1.name}");
        characterManager.UpdateCharacterPosition(theCharacter);

        //Tell turn controller to update initiative for this character
        if (debugLogging) print($"Tell turn controller to update initiative for this character for {theCharacter.Item2.Name}");
        turnManager.UpdateInitiativeTracker(theCharacter.Item2);

        //Remove selections
        if (debugLogging) print($"Removing selections and highlighted tiles");
        selectionManager.RemoveSelections();
        selectionManager.ResetHighlightedTiles();
    }

    internal Dictionary<(int, int), GameObject> GetMoves(ValueTuple<GameObject, Character> theCharacter)
    {
        //todo write version of pathfinder that gets all legal moves up to MA away
        int movementAllowance = theCharacter.Item2.MA;
        var currentX = theCharacter.Item1.transform.position.x;
        var currentZ = theCharacter.Item1.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid();
        var furnitureArray = dungeonManager.GetFurnitureArray();
        Dictionary<(int, int), GameObject> possibleMoves = new Dictionary<(int, int), GameObject>();

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        var upperSquare = dungeonGrid.Where(a => a.Key == (currentX + 1, currentZ)).FirstOrDefault();
        var lowerSquare = dungeonGrid.Where(a => a.Key == (currentX - 1, currentZ)).FirstOrDefault();
        var leftSquare = dungeonGrid.Where(a => a.Key == (currentX, currentZ + 1)).FirstOrDefault();
        var rightSquare = dungeonGrid.Where(a => a.Key == (currentX, currentZ - 1)).FirstOrDefault();

        var result = CheckSquareForPossibleMove(furnitureArray, possibleMoves, upperSquare);

        if (result.Item2 != null)
        {
            possibleMoves.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleMove(furnitureArray, possibleMoves, lowerSquare);

        if (result.Item2 != null)
        {
            possibleMoves.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleMove(furnitureArray, possibleMoves, leftSquare);

        if (result.Item2 != null)
        {
            possibleMoves.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        result = CheckSquareForPossibleMove(furnitureArray, possibleMoves, rightSquare);

        if (result.Item2 != null)
        {
            possibleMoves.Add((result.Item1.Item1, result.Item1.Item2), result.Item2);
        }

        return possibleMoves;
    }

    private ValueTuple<(int, int), GameObject> CheckSquareForPossibleMove(GameObject[][] furnitureArray, Dictionary<(int, int), GameObject> possibleMoves, KeyValuePair<(int, int), GameObject> squareToCheck)
    {
        if (squareToCheck.Value != null)
        {
            //see if the square is occupied: (check character lists, furniture list)
            var characterInPosition = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);
            var furnitureInPosition = furnitureArray[squareToCheck.Key.Item1][squareToCheck.Key.Item2];

            //if not, add them to move list
            if (characterInPosition.Item1 == null && furnitureInPosition == null)
            {
                return ((squareToCheck.Key.Item1, squareToCheck.Key.Item2), squareToCheck.Value);
            }
        }

        return ((0, 0), null);
    }
}

