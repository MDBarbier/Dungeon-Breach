using System;
using System.Collections;
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

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject dungetonTile;
    [SerializeField] Material dungeonTileColour1;
    [SerializeField] Material dungeonTileColour2;
    [SerializeField] GameObject characterPiece;
    [SerializeField] Material blueTeamCharacterColour;
    [SerializeField] Material redTeamCharacterColour;
    [SerializeField] GameObject dungeonFurniture;
    [SerializeField] Material dungeonFurnitureMaterial;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        gridPositions = new Dictionary<(int, int), GameObject>();
        characterList = new Dictionary<GameObject, Character>();
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
        InitializeJaggedArray(xlength, zlength);
        AddFurniture();

        if (gridGenerator == null)
        {
            Debug.LogError("DungeonManager fatal error: Could not locate the grid generator");
            return;
        }

        //Generate grid and assign output to grid positions
        gridPositions = gridGenerator.RenderGrid(zlength, xlength, dungetonTile);

        //Assign materials to grid tiles
        gridGenerator.ApplyMaterials(dungeonTileColour1, dungeonTileColour2, xlength, zlength, gridPositions);

        //Instantiate demo characters
        var boromir = InstantiateCharacter("Boromir", 10, 8, 20, 10, 25, 14, true, new Vector3(1f, 0.75f, 0f), blueTeamCharacterColour);
        characterList.Add(boromir.Item2, boromir.Item1);
        var gimli = InstantiateCharacter("Gimli", 8, 20, 12, 18, 50, 10, true, new Vector3(2f, 0.75f, 0f), blueTeamCharacterColour);
        characterList.Add(gimli.Item2, gimli.Item1);
        var aragorn = InstantiateCharacter("Aragorn", 8, 20, 12, 18, 50, 10, true, new Vector3(3f, 0.75f, 0f), blueTeamCharacterColour);
        characterList.Add(aragorn.Item2, aragorn.Item1);

        var orc1 = InstantiateCharacter("Orc1", 8, 20, 12, 18, 50, 10, false, new Vector3(0f, 0.75f, 4f), redTeamCharacterColour);
        characterList.Add(orc1.Item2, orc1.Item1);
        var orc2 = InstantiateCharacter("Orc2", 8, 20, 12, 18, 50, 10, false, new Vector3(1f, 0.75f, 4f), redTeamCharacterColour);
        characterList.Add(orc2.Item2, orc2.Item1);
        var orc3 = InstantiateCharacter("Orc3", 8, 20, 12, 18, 50, 10, false, new Vector3(2f, 0.75f, 4f), redTeamCharacterColour);
        characterList.Add(orc3.Item2, orc3.Item1);
        var orc4 = InstantiateCharacter("Orc4", 8, 20, 12, 18, 50, 10, false, new Vector3(3f, 0.75f, 4f), redTeamCharacterColour);
        characterList.Add(orc4.Item2, orc4.Item1);
        var orc5 = InstantiateCharacter("Orc5", 8, 20, 12, 18, 50, 10, false, new Vector3(4f, 0.75f, 4f), redTeamCharacterColour);
        characterList.Add(orc5.Item2, orc5.Item1);

        //Print the location of all characters to the log
        foreach (var characterInPlay in characterList)
        {
            print($"{characterInPlay.Value.Name} is in square X: {characterInPlay.Key.transform.position.x}, Z: {characterInPlay.Key.transform.position.z}");
        }
    }

    private (Character, GameObject) InstantiateCharacter(string name, int cha, int str, int dex, int con, int hp, int intelligence, bool playercontrolled, Vector3 coordinatesToCreateAt, Material material)
    {
        var character = new Character() { Name = name, CHA = cha, CON = con, DEX = dex, HP = hp, INT = intelligence, PlayerControlled = playercontrolled, STR = str };
        characterPiece.name = character.Name;        
        var charGo = Instantiate(characterPiece, coordinatesToCreateAt, Quaternion.identity);
        var mesh = charGo.GetComponent<MeshRenderer>();
        mesh.material = material;
        return (character, charGo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //MVP only
    private void AddFurniture()
    {
        var renderer = dungeonFurniture.GetComponent<MeshRenderer>();
        renderer.material = dungeonFurnitureMaterial;

        //add a wall down the middle of the map with a gap in it to act as a chokepoint
        furniturePositions[0][2] = Instantiate(dungeonFurniture, new Vector3(0, 0.65f, 2), Quaternion.identity);
        furniturePositions[1][2] = Instantiate(dungeonFurniture, new Vector3(1, 0.65f, 2), Quaternion.identity);
        furniturePositions[2][2] = Instantiate(dungeonFurniture, new Vector3(2, 0.65f, 2), Quaternion.identity);
        furniturePositions[4][2] = Instantiate(dungeonFurniture, new Vector3(4, 0.65f, 2), Quaternion.identity);

        PrintJaggedArray(furniturePositions);
    }

    //Reusable
    private void InitializeJaggedArray(int xaxis, int zaxis)
    {
        for (int i = 0; i < xaxis; i++)
        {
            furniturePositions[i] = new GameObject[zaxis];
        }
    }

    //Reusable
    private void PrintJaggedArray(GameObject[][] gameObjectsArray)
    {
        var outerLength = gameObjectsArray.Length;

        for (int i = 0; i < outerLength; i++)
        {
            var innerLength = gameObjectsArray[i].Length;

            for (int j = 0; j < innerLength; j++)
            {
                if (gameObjectsArray[i][j] != null)
                {
                    print($"X: {i}, Z: {j}, Name: {gameObjectsArray[i][j].name}");
                }
            }
        }
    }

}

public class Character
{
    public string Name { get; set; }
    public int STR { get; set; }
    public int CON { get; set; }
    public int INT { get; set; }
    public int DEX { get; set; }
    public int CHA { get; set; }
    public int HP { get; set; }
    public bool PlayerControlled { get; set; }
}
