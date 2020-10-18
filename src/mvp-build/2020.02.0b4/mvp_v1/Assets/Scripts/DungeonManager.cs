using Assets.Scripts.Classes;
using Assets.Scripts.Nonmonobehaviour;
using Assets.Scripts.NonMonoBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private GridGenerator gridGenerator;    
    private GameObject[][] furniturePositions;
    private Dictionary<GameObject, Character> characterList;
    private Dictionary<(int, int), GameObject> gridPositions;
    private bool hasInitialisedGrid;
    private bool hasInitialisedFurniture;
    private bool hasInitialisedCharacters;
    private DiceRoller diceRoller;
    private CharacterManager characterManager;
    private TurnManager turnManager;
    private CombatLogHandler combatLogHandler;
    private System.Random random;

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
    [SerializeField] int xlength = 5;
    [SerializeField] int zlength = 5;
    [SerializeField] int numberOfPlayerCharacters;
    [SerializeField] int numberOfEnemies;
    [SerializeField] Vector3 playerStartingPoint;
    [SerializeField] Vector3 enemyStartingPoint;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        InitialiseObjects();
        FindInstances();
    }

    private void InitialiseObjects()
    {
        furniturePositions = new GameObject[zlength][];
        gridPositions = new Dictionary<(int, int), GameObject>();
        characterList = new Dictionary<GameObject, Character>();
        diceRoller = new DiceRoller();
        random = new System.Random();
    }

    private void FindInstances()
    {
        turnManager = FindObjectOfType<TurnManager>();
        gridGenerator = FindObjectOfType<GridGenerator>();
        characterManager = FindObjectOfType<CharacterManager>();
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

            for (int i = 0; i < numberOfPlayerCharacters; i++)
            {                
                var newFighter = new Fighter();
                newFighter.Race = Assets.Scripts.Enums.Races.Human;
                newFighter.PlayerControlled = true;
                newFighter.SetRandomName(random, characterManager.GetAllCharacters());

                var spaceToInstantiate = FindSpaceToInstantiate(playerStartingPoint);
                characterManager.InstantiateCharacter(newFighter, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), blueTeamCharacterColour, characterPiece);
            }

            for (int i = 0; i < numberOfEnemies; i++)
            {                
                var newBandit = new Bandit();                
                newBandit.Race = Assets.Scripts.Enums.Races.Orc;                
                newBandit.SetRandomName(random, characterManager.GetAllCharacters());

                var spaceToInstantiate = FindSpaceToInstantiate(enemyStartingPoint);
                characterManager.InstantiateCharacter(newBandit, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), redTeamCharacterColour, characterPiece);
            }

            turnManager.SetInitiative(characterManager.GetAllCharacters());

            combatLogHandler.CombatLog($"There are {characterManager.GetCpuCharacters().Count} enemies lurking in this dungoen");
                        
            if (debugLogging)
            {
                characterManager.PrintCharacters(); 
            }
        }
    }

    internal List<(int, int)> CheckSurroundingSquares(Vector3 startPoint, int offset)
    {        
        var currentX = startPoint.x;
        var currentZ = startPoint.z;
        var dungeonGrid = GetDungeonGrid();
        var furnitureArray = GetFurnitureArray();
        List<(int, int)> possibilities = new List<(int, int)>();

        //via dungeon manager find the game objects for the 4 squares around the current square (if they exist)
        List<KeyValuePair<(int, int), GameObject>> listToCheck = new List<KeyValuePair<(int, int), GameObject>>();

        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + offset, currentZ)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - offset, currentZ)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ + offset)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX, currentZ - offset)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + offset, currentZ + offset)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX + offset, currentZ - offset)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - offset, currentZ + offset)).FirstOrDefault());
        listToCheck.Add(dungeonGrid.Where(a => a.Key == (currentX - offset, currentZ - offset)).FirstOrDefault());

        foreach (var squareToCheck in listToCheck)
        {
            if (squareToCheck.Value == null)
            {
                continue;
            }

            var characterAtPosition = characterManager.GetCharacterAtPosition(squareToCheck.Key.Item1, squareToCheck.Key.Item2);
            var furnitureInPosition = furniturePositions[squareToCheck.Key.Item1][squareToCheck.Key.Item2];

            if (characterAtPosition.Item1 == null && furnitureInPosition == null)
            {
                possibilities.Add((squareToCheck.Key.Item1, squareToCheck.Key.Item2));
            }
        }

        return possibilities;
    }


    private Vector3 FindSpaceToInstantiate(Vector3 startPoint)
    {
        for (int i = 0; i < xlength; i++)
        {
            //Check the spaces directly around the startPoint
            var possibilities = CheckSurroundingSquares(startPoint, i);

            if (possibilities.Count > 0)
            {
                return new Vector3(possibilities.First().Item1, startPoint.y, possibilities.First().Item2);
            } 
        }

        throw new Exception("Unable to find a space to initialise!");
    }

    private void AddFurniture()
    {
        //get material for the crates
        var renderer = dungeonFurniture.GetComponent<MeshRenderer>();
        renderer.material = dungeonFurnitureMaterial;

        System.Random rand = new System.Random();
        
        var numberOfObjects = rand.Next(xlength, xlength*2);

        for (int i = 0; i < numberOfObjects; i++)
        {
            var xPos = rand.Next(xlength);
            var zPos = rand.Next(zlength);

            if (furniturePositions[xPos][zPos] == null)
            {
                furniturePositions[xPos][zPos] = Instantiate(dungeonFurniture, new Vector3(xPos, 0.65f, zPos), Quaternion.identity);
            }
        }

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
