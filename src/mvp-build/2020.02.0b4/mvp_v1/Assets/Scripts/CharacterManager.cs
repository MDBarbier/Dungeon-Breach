using Assets.Scripts.Classes;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private Dictionary<GameObject, Character> playerCharacterList;
    private Dictionary<GameObject, Character> enemyList;

    // Start is called before the first frame update
    void Start()
    {
        enemyList = new Dictionary<GameObject, Character>();
        playerCharacterList = new Dictionary<GameObject, Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal (Character, GameObject) InstantiateCharacter(string name, int cha, int str, int dex, int con, int hp, int intelligence, bool playercontrolled, Vector3 coordinatesToCreateAt, Material material, GameObject characterPiece)
    {
        var character = new Character() { Name = name, CHA = cha, CON = con, DEX = dex, HP = hp, INT = intelligence, PlayerControlled = playercontrolled, STR = str };                
        var charGo = Instantiate(characterPiece, coordinatesToCreateAt, Quaternion.identity);
        charGo.name = name;        
        var mesh = charGo.GetComponent<MeshRenderer>();
        mesh.material = material;

        if (playercontrolled)
        {
            playerCharacterList.Add(charGo, character); 
        }
        else
        {
            enemyList.Add(charGo, character);
        }

        return (character, charGo);
    }

    internal void PrintCharacters()
    {
        foreach (var characterInPlay in playerCharacterList)
        {
            print($"Player character {characterInPlay.Value.Name} is in square X: {characterInPlay.Key.transform.position.x}, Z: {characterInPlay.Key.transform.position.z}");
        }

        foreach (var characterInPlay in enemyList)
        {
            print($"CPU character {characterInPlay.Value.Name} is in square X: {characterInPlay.Key.transform.position.x}, Z: {characterInPlay.Key.transform.position.z}");
        }
    }
}
