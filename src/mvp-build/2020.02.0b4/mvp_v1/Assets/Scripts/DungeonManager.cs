using Assets.Scripts.Classes;
using Assets.Scripts.Enums;
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
    private GamePersistenceEngine gamePersistenceEngine;
    private int numberOfPlayerMeleeCharacters;
    private int numberOfPlayerHealers;
    private int numberOfPlayerRangedCharacters;
    private int numberOfMeleeEnemies;
    private int numberOfRangedEnemies;
    private int xlength;
    private int zlength;
    private Vector3 playerStartingPoint;
    private Vector3 enemyStartingPoint;
    private int numberOfFurniturePieces;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject dungetonTile;
    [SerializeField] Material dungeonTileColour1;
    [SerializeField] Material dungeonTileColour2;
    [SerializeField] GameObject characterPiece;
    [SerializeField] Material blueTeamCharacterColour;
    [SerializeField] Material redTeamCharacterColour;
    [SerializeField] Material blueTeamCharacterColour2;
    [SerializeField] Material blueTeamCharacterColour3;
    [SerializeField] Material redTeamCharacterColour2;
    [SerializeField] GameObject dungeonFurniture;
    [SerializeField] Material dungeonFurnitureMaterial;
    [SerializeField] bool debugLogging = false;
    [SerializeField] bool overridePersistence = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {   
        FindInstances();
        LoadDataFromGamePersistenceEngine();
        InitialiseObjects();
    }

    private void LoadDataFromGamePersistenceEngine()
    {
        if (!overridePersistence)
        {
            var battleState = gamePersistenceEngine.BattleState;
            numberOfPlayerMeleeCharacters = battleState.PlayerMeleeCharacters;
            numberOfPlayerRangedCharacters = battleState.PlayerRangedCharacters;
            numberOfPlayerHealers = battleState.PlayerHealerCharacters;
            numberOfMeleeEnemies = battleState.EnemyMeleeCharacters;
            numberOfRangedEnemies = battleState.EnemyRangedCharacters;
            zlength = battleState.RoomZ;
            xlength = battleState.RoomX;
            playerStartingPoint = new Vector3(0, 0, 0);
            enemyStartingPoint = new Vector3(xlength - 2, 0, zlength - 2);
            numberOfFurniturePieces = (xlength + zlength / 2);
        }
        else
        {
            var battleState = new BattleState()
            {
                EnemyMeleeCharacters = 1,
                EnemyRangedCharacters = 0,
                PlayerMeleeCharacters = 1,
                PlayerHealerCharacters = 0,
                PlayerRangedCharacters = 0,                
                RoomX = 10,
                RoomZ = 10                
            };

            numberOfPlayerMeleeCharacters = battleState.PlayerMeleeCharacters;
            numberOfPlayerRangedCharacters = battleState.PlayerRangedCharacters;
            numberOfPlayerHealers = battleState.PlayerHealerCharacters;
            numberOfMeleeEnemies = battleState.EnemyMeleeCharacters;
            numberOfRangedEnemies = battleState.EnemyRangedCharacters;
            zlength = battleState.RoomZ;
            xlength = battleState.RoomX;
            playerStartingPoint = new Vector3(0, 0, 0);
            enemyStartingPoint = new Vector3(xlength - 2, 0, zlength - 2);
            numberOfFurniturePieces = 12;
        }
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
        gamePersistenceEngine = FindObjectOfType<GamePersistenceEngine>();
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

            for (int i = 0; i < numberOfPlayerHealers; i++)
            {
                var newChar = new MagicCharacter();
                newChar.SetDamageType(DamageTypes.Healing);
                newChar.Race = Races.Human;
                newChar.PlayerControlled = true;
                newChar.SetRandomName(random, characterManager.GetAllCharacters());
                newChar.Subclass = Subclasses.Healer;
                var spaceToInstantiate = FindSpaceToInstantiate(playerStartingPoint);
                characterManager.InstantiateCharacter(newChar, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), blueTeamCharacterColour3, characterPiece);
            }

            for (int i = 0; i < numberOfPlayerRangedCharacters; i++)
            {
                var newChar = new RangedCharacter();
                newChar.Race = Races.Human;                
                newChar.PlayerControlled = true;
                newChar.SetRandomName(random, characterManager.GetAllCharacters());
                newChar.Subclass = Subclasses.Archer;
                var spaceToInstantiate = FindSpaceToInstantiate(playerStartingPoint);
                characterManager.InstantiateCharacter(newChar, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), blueTeamCharacterColour2, characterPiece);
            }

            for (int i = 0; i < numberOfPlayerMeleeCharacters; i++)
            {
                var newChar = new MeleeCharacter();
                newChar.Race = Races.Human;                
                newChar.PlayerControlled = true;
                newChar.SetRandomName(random, characterManager.GetAllCharacters());
                newChar.Subclass = Subclasses.Fighter;
                var spaceToInstantiate = FindSpaceToInstantiate(playerStartingPoint);
                characterManager.InstantiateCharacter(newChar, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), blueTeamCharacterColour, characterPiece);
            }

            for (int i = 0; i < numberOfRangedEnemies; i++)
            {
                var newChar = new RangedCharacter();
                newChar.Race = Races.Orc;
                newChar.SetRandomName(random, characterManager.GetAllCharacters());
                newChar.Subclass = Subclasses.Bandit;
                var spaceToInstantiate = FindSpaceToInstantiate(enemyStartingPoint);
                characterManager.InstantiateCharacter(newChar, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), redTeamCharacterColour2, characterPiece);
            }

            for (int i = 0; i < numberOfMeleeEnemies; i++)
            {                
                var newChar = new MeleeCharacter();
                newChar.Race = Races.Orc;                
                newChar.SetRandomName(random, characterManager.GetAllCharacters());
                newChar.Subclass = Subclasses.Bandit;
                var spaceToInstantiate = FindSpaceToInstantiate(enemyStartingPoint);
                characterManager.InstantiateCharacter(newChar, new Vector3(spaceToInstantiate.x, 0.75f, spaceToInstantiate.z), redTeamCharacterColour, characterPiece);
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
        
        var numberOfObjects = numberOfFurniturePieces;

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

    internal bool IsObstacleInSpace(Vector3 space)
    {
        try
        {
            if (furniturePositions[(int)space.x][(int)space.z] != null)
            {
                return true;
            }
            else { return false; }
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    internal GameObject GetFloorTileByLocation(float x, float z)
    {
        return gridPositions.Where(a => a.Key.Item1 == x && a.Key.Item2 == z).FirstOrDefault().Value;
    }

}
