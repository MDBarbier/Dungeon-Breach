using Assets.Scripts.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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

    internal Dictionary<GameObject, Character> GetPlayerCharacters()
    {
        return playerCharacterList;
    }

    internal Dictionary<GameObject, Character> GetCpuCharacters()
    {
        return enemyList;
    }

    internal (GameObject, Character) GetPlayerCharacter(string gameObjectName)
    {
        var goMatch = playerCharacterList.Where(c => c.Key.name == gameObjectName).Select(c => c).FirstOrDefault();

        if (goMatch.Key != null)
        {
            return (goMatch.Key, goMatch.Value);
        }
        else 
        {
            return (null, null);
        }
    }

    internal (GameObject, Character) GetCpuCharacter(string gameObjectName)
    {
        var goMatch = enemyList.Where(c => c.Key.name == gameObjectName).Select(c => c).FirstOrDefault();

        if (goMatch.Key != null)
        {
            return (goMatch.Key, goMatch.Value);
        }
        else
        {
            return (null, null);
        }
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

    internal (GameObject, Character) GetCharacterAtPosition(int xPosition, int zPosition)
    {
        var matchPlayerChar = playerCharacterList.Where(a => a.Key.transform.position.x == xPosition && a.Key.transform.position.z == zPosition).FirstOrDefault();
        var matchCpuChar = enemyList.Where(a => a.Key.transform.position.x == xPosition && a.Key.transform.position.z == zPosition).FirstOrDefault();

        if (matchCpuChar.Key != null && matchPlayerChar.Key != null)
        {
            throw new System.Exception("Two characters should never be in the same square!");
        }
        else if (matchCpuChar.Key != null && matchPlayerChar.Key == null)
        {
            return (matchCpuChar.Key, matchCpuChar.Value);
        }
        else if (matchPlayerChar.Key != null && matchCpuChar.Key == null)
        {
            return (matchPlayerChar.Key, matchPlayerChar.Value);
        }
        else
        {
            return (null, null);
        }
    }

    internal void UpdateCharacterPosition(ValueTuple<GameObject, Character> theCharacter)
    {
        //Find the character in either the player list or cpu list
        var matchPlayerChar = playerCharacterList.Where(a => a.Key == theCharacter.Item1).FirstOrDefault();
        var matchCpuChar = enemyList.Where(a => a.Key == theCharacter.Item1).FirstOrDefault();

        if (matchCpuChar.Key != null && matchPlayerChar.Key != null)
        {
            throw new System.Exception("Two characters should never be in the same list!");
        }
        else if (matchCpuChar.Key != null && matchPlayerChar.Key == null)
        {
            //update the cpu char
            enemyList.Remove(matchCpuChar.Key);
            enemyList.Add(theCharacter.Item1, theCharacter.Item2);
        }
        else if (matchPlayerChar.Key != null && matchCpuChar.Key == null)
        {
            //update the player char
            playerCharacterList.Remove(matchPlayerChar.Key);
            playerCharacterList.Add(theCharacter.Item1, theCharacter.Item2);
        }
        else
        {
            throw new Exception($"No character found with the supplied parameters: {theCharacter.Item1.name}");
        }

    }

    internal List<Character> GetAllCharacters()
    {
        List<Character> characters = new List<Character>();

        foreach (var character in playerCharacterList)
        {
            characters.Add(character.Value);
        }

        foreach (var character in enemyList)
        {
            characters.Add(character.Value);
        }

        return characters;
    }
}
