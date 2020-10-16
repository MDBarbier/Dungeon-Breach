using Assets.Scripts.Classes;
using Assets.Scripts.Nonmonobehaviour;
using Assets.Scripts.NonMonoBehaviour;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private GridGenerator gridGenerator;
    private const int xlength = 5;
    private const int zlength = 5;
    private GameObject[][] furniturePositions = new GameObject[zlength][];
    private Dictionary<GameObject, Character> characterList;
    private Dictionary<(int, int), GameObject> gridPositions;
    private bool hasInitialisedGrid;
    private bool hasInitialisedFurniture;
    private bool hasInitialisedCharacters;
    private DiceRoller diceRoller;
    private CharacterManager characterManager;
    private TurnManager turnManager;
    private CombatLogHandler combatLogHandler;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject dungetonTile;
    [SerializeField] Material dungeonTileColour1;
    [SerializeField] Material dungeonTileColour2;
    [SerializeField] GameObject characterPiece;
    [SerializeField] Material blueTeamCharacterColour;
    [SerializeField] Material redTeamCharacterColour;
    [SerializeField] GameObject dungeonFurniture;
    [SerializeField] Material dungeonFurnitureMaterial;
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        gridPositions = new Dictionary<(int, int), GameObject>();
        characterList = new Dictionary<GameObject, Character>();
        gridGenerator = FindObjectOfType<GridGenerator>();
        characterManager = FindObjectOfType<CharacterManager>();
        diceRoller = new DiceRoller();
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
    }    

    // Update is called once per frame
    void Update()
    {
        InitialiseGrid();

        InitialiseFurniture();

        InitialiseCharacters();
    }

    private void InitialiseGrid()
    {
        if (!hasInitialisedGrid)
        {
            hasInitialisedGrid = true;

            if (gridGenerator == null)
            {
                Debug.LogError("DungeonManager fatal error: Could not locate the grid generator");
                return;
            }

            //Generate grid and assign output to grid positions
            gridPositions = gridGenerator.RenderGrid(zlength, xlength, dungetonTile);

            //Assign materials to grid tiles
            gridGenerator.ApplyMaterials(dungeonTileColour1, dungeonTileColour2, xlength, zlength, gridPositions);

        }
    }

    private void InitialiseFurniture()
    {
        if (!hasInitialisedFurniture)
        {
            hasInitialisedFurniture = true;

            furniturePositions = JaggedArrayMethods.InitializeJaggedArray(xlength, zlength, furniturePositions);
            AddFurniture();
        }
    }

    private void InitialiseCharacters()
    {
        if (!hasInitialisedCharacters)
        {
            hasInitialisedCharacters = true;

            //Instantiate demo characters
            characterManager.InstantiateCharacter("Boromir", 10, 17, 15, 15, 50, 14, 17, true, new Vector3(2f, 0.75f, 0f), blueTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Gimli", 8, 20, 12, 18, 64, 10, 19, true, new Vector3(3f, 0.75f, 0f), blueTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Aragorn", 16, 16, 16, 17, 55, 15, 18, true, new Vector3(4f, 0.75f, 0f), blueTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Orc1", 8, 15, 10, 18, 20, 10, 12, false, new Vector3(0f, 0.75f, 4f), redTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Orc2", 8, 15, 10, 18, 20, 10, 12, false, new Vector3(1f, 0.75f, 4f), redTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Orc3", 8, 15, 10, 18, 20, 10, 12, false, new Vector3(2f, 0.75f, 4f), redTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Orc4", 8, 15, 10, 18, 20, 10, 12, false, new Vector3(3f, 0.75f, 4f), redTeamCharacterColour, characterPiece);
            characterManager.InstantiateCharacter("Orc5", 8, 15, 10, 18, 20, 10, 12, false, new Vector3(4f, 0.75f, 4f), redTeamCharacterColour, characterPiece);

            turnManager.SetInitiative(characterManager.GetAllCharacters());

            combatLogHandler.CombatLog($"There are {characterManager.GetCpuCharacters().Count} enemies lurking in this dungoen");

            //Print the location of all characters to the log
            if (debugLogging)
            {
                characterManager.PrintCharacters(); 
            }
        }
    }
        
    private void AddFurniture()
    {
        //get material for the crates
        var renderer = dungeonFurniture.GetComponent<MeshRenderer>();
        renderer.material = dungeonFurnitureMaterial;

        //add a wall down the middle of the map with a gap in it to act as a chokepoint
        furniturePositions[0][2] = Instantiate(dungeonFurniture, new Vector3(0, 0.65f, 2), Quaternion.identity);
        furniturePositions[1][2] = Instantiate(dungeonFurniture, new Vector3(1, 0.65f, 2), Quaternion.identity);
        furniturePositions[2][2] = Instantiate(dungeonFurniture, new Vector3(2, 0.65f, 2), Quaternion.identity);
        furniturePositions[4][2] = Instantiate(dungeonFurniture, new Vector3(4, 0.65f, 2), Quaternion.identity);

        JaggedArrayMethods.AddTagToJaggedArrayOfGameObjects(furniturePositions, "Scenery");
        JaggedArrayMethods.SetParentOfJaggedArrayGameObjects(furniturePositions, "InstantiatedFurniture");
        //log the jagged array to console
        if (debugLogging)
        {
            print(JaggedArrayMethods.GetJaggedArrayOutputString(furniturePositions)); 
        }
    }       

    internal Dictionary<(int, int), GameObject> GetDungeonGrid()
    {
        return gridPositions;
    }

    internal GameObject[][] GetFurnitureArray()
    {
        return furniturePositions;
    }

}
