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
    private Dictionary<(int, int), (GameObject, Material)> highlightedTiles;
    private GameObject lastSelectedGameObject;
    private TurnManager turnManager;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] Material dungetonTileSelected;    
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        ResetHighlightedTiles();
        dungeonManager = FindObjectOfType<DungeonManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionManager.selectedCharacter.Item1 != null)
        {
            if (lastSelectedGameObject == selectionManager.selectedCharacter.Item1)
            {
                return; //only continue if the selection has changed
            }

            //return highlighted tiles to their normal state
            foreach (var tile in highlightedTiles)
            {
                tile.Value.Item1.GetComponent<MeshRenderer>().material = tile.Value.Item2;
            }

            ResetHighlightedTiles();

            var possibleMoves = GetMoves(selectionManager.selectedCharacter);

            foreach (var tile in possibleMoves)
            {
                highlightedTiles.Add((tile.Key.Item1, tile.Key.Item2), (tile.Value, tile.Value.GetComponent<MeshRenderer>().material));
                tile.Value.GetComponent<MeshRenderer>().material = dungetonTileSelected;
            }

            //assign the gameobject we just processed to the last game object flag
            lastSelectedGameObject = selectionManager.selectedCharacter.Item1;
        }
        else
        {
            //return highlighted tiles to their normal state
            foreach (var tile in highlightedTiles)
            {
                tile.Value.Item1.GetComponent<MeshRenderer>().material = tile.Value.Item2;
            }

            ResetHighlightedTiles();
            lastSelectedGameObject = null;
        }
    }

    private void ResetHighlightedTiles()
    {
        if (highlightedTiles != null)
        {
            highlightedTiles.Clear(); 
        }
        highlightedTiles = new Dictionary<(int, int), (GameObject, Material)>();
    }

    internal void MoveCharacter(ValueTuple<GameObject, Character> theCharacter, ValueTuple<(int, int), GameObject> squareToMoveTo)
    {
        //Perform the physical move of the gameobject
        theCharacter.Item1.transform.localPosition = new Vector3(squareToMoveTo.Item1.Item1, theCharacter.Item1.transform.position.y, squareToMoveTo.Item1.Item2);

        //Report the change back to the character controller
        characterManager.UpdateCharacterPosition(theCharacter);

        //Tell turn controller to update initiative for this character
        turnManager.UpdateInitiativeTracker(theCharacter.Item2);

        //Remove selections
        selectionManager.RemoveSelections();
    }

    internal Dictionary<(int, int), GameObject> GetMoves(ValueTuple<GameObject, Character> theCharacter)
    {
        int movementAllowance = theCharacter.Item2.MA;
        var currentX = theCharacter.Item1.transform.position.x;
        var currentZ = theCharacter.Item1.transform.position.z;
        var dungeonGrid = dungeonManager.GetDungeonGrid(); //todo refactor into getter on dungeon manager
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

    internal Dictionary<(int, int), (GameObject, Material)> GetHighlightedTiles() => highlightedTiles;

}

