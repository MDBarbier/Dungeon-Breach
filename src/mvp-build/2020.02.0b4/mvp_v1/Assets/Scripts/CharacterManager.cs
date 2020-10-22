using Assets.Scripts.Classes;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CharacterManager : MonoBehaviour
{
    private Dictionary<GameObject, Character> playerCharacterList;
    private Dictionary<GameObject, Character> enemyList;
    private TurnManager turnManager;
    private CombatLogHandler combatLogHandler;
    private GamePersistenceEngine gamePersistenceEngine;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject textMesh;    
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        enemyList = new Dictionary<GameObject, Character>();
        playerCharacterList = new Dictionary<GameObject, Character>();
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
        gamePersistenceEngine = FindObjectOfType<GamePersistenceEngine>();
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

    internal void ApplyDamage(Character target, int damageDealt)
    {
        if (target.PlayerControlled)
        {
            var match = playerCharacterList.Where(a => a.Value.Name == target.Name).FirstOrDefault();

            match.Value.HP -= damageDealt;

            DisplayDamageText(damageDealt, match.Key);

            if (match.Value.HP <= 0)
            {
                combatLogHandler.CombatLog($"{target.Name} is slain!");
                var go = GetCharacterGameObject(target);

                if (go == null)
                {
                    throw new Exception($"ApplyDamage method error: character {target.Name} could not be found!");
                }

                playerCharacterList.Remove(go);
                Destroy(go);
                turnManager.RemoveCharacterFromInitiative(target);
                gamePersistenceEngine.AlterPlayerCharacterCount(-1);
            }
            else if (match.Value.HP > match.Value.MAXHP)
            {
                match.Value.HP = match.Value.MAXHP;
                combatLogHandler.CombatLog($"{target.Name} is fully healed");
            }
            else
            {
                combatLogHandler.CombatLog($"{target.Name} has {target.HP} hit points left");
            }

        }
        else
        {
            var match = enemyList.Where(a => a.Value.Name == target.Name).FirstOrDefault();

            match.Value.HP -= damageDealt;
            DisplayDamageText(damageDealt, match.Key);

            if (match.Value.HP <= 0)
            {
                combatLogHandler.CombatLog($"{target.Name} is slain!");
                var go = GetCharacterGameObject(target);

                if (go == null)
                {
                    throw new Exception($"ApplyDamage method error: character {target.Name} could not be found!");
                }

                enemyList.Remove(go);
                Destroy(go);
                turnManager.RemoveCharacterFromInitiative(target);
            }

        }
    }

    private void DisplayDamageText(int damageDealt, GameObject target)
    {
        //instantiate a text mesh prefab "FloatingText"
        var textMeshGameObject = Instantiate(textMesh, target.transform.position, Quaternion.identity);

        //make it face the camera        
        textMeshGameObject.transform.rotation = Camera.main.transform.rotation;

        //Set parent
        var parent = GameObject.Find(target.name);
        textMeshGameObject.transform.parent = parent.transform;
        var position = parent.transform.position;
        position.y = position.y + 0.75f;        
        textMeshGameObject.transform.position = position;
        textMeshGameObject.transform.position = textMeshGameObject.transform.position + textMeshGameObject.transform.right * -0.75f;

        //Set the text to the damage amount
        var mesh = textMeshGameObject.GetComponent<TextMesh>();
        mesh.text = $"{damageDealt} Damage";

        //Set text mesh go to be destroyed after x seconds
        var destroyAfterTime = textMeshGameObject.GetComponent<DestroyAfterTime>();
        destroyAfterTime.Invoke(nameof(DestroyAfterTime.Destroy), 3f);
    }    

    internal GameObject InstantiateCharacter(Character character, Vector3 coordinatesToCreateAt, Material material, GameObject characterPiece)
    {
        //check the name is not in use
        var matchP = GetCharacterByName(character.Name);
        var matchE = GetCharacterByName(character.Name);
        
        if (!string.IsNullOrWhiteSpace(matchP.Name) || !string.IsNullOrWhiteSpace(matchE.Name))
        {
            throw new Exception("Cannot add a character with the same name: " + character.Name);
        }

        var charGo = Instantiate(characterPiece, coordinatesToCreateAt, Quaternion.identity);
        
        //Set attributes on the gameobject
        charGo.name = character.Name;
        charGo.tag = "Character";
        var parent = GameObject.Find("InstantiatedCharacters");
        charGo.transform.parent = parent.transform;
        var mesh = charGo.GetComponent<MeshRenderer>();
        mesh.material = material;

        if (character.PlayerControlled)
        {
            playerCharacterList.Add(charGo, character);
        }
        else
        {
            enemyList.Add(charGo, character);
        }

        return charGo;
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

    internal Character GetCharacterByName(string name)
    {
        var matchP = playerCharacterList.Where(a => a.Value.Name == name).FirstOrDefault();
        var matchE = enemyList.Where(a => a.Value.Name == name).FirstOrDefault();

        if (matchP.Value != null)
        {
            if (!string.IsNullOrWhiteSpace(matchP.Value.Name))
            {
                return matchP.Value;
            }
        }

        if (matchE.Value != null)
        {
            if (!string.IsNullOrWhiteSpace(matchE.Value.Name))
            {
                return matchE.Value;
            }
        }

        //If we get here there is no match so return empty character
        return new Character();

    }    

    internal GameObject GetCharacterGameObject(Character character)
    {
        if (character == null)
        {
            throw new Exception("Null character sent to GetCharacterGameObject");
        }

        var matchP = playerCharacterList.Where(a => a.Value.Name == character.Name).FirstOrDefault();
        var matchE = enemyList.Where(a => a.Value.Name == character.Name).FirstOrDefault();

        if (matchP.Value != null)
        {
            if (!string.IsNullOrWhiteSpace(matchP.Value.Name))
            {
                return matchP.Key;
            }
        }

        if (matchE.Value != null)
        {
            if (!string.IsNullOrWhiteSpace(matchE.Value.Name))
            {
                return matchE.Key;
            }
        }

        //If we get here there is no match so return empty character
        throw new Exception("No character found");
    }
}
