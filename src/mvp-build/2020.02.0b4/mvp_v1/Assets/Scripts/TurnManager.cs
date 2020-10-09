using Assets.Scripts.Classes;
using Assets.Scripts.NonMonoBehaviour;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] bool playersTurn = true;
    [SerializeField] string nextCharacterToAct;
#pragma warning restore 649

    private DiceRoller diceRoller;
    private Dictionary<Character, int> initiativeTracker;
    private HashSet<Character> actedCharacters;
    private CharacterManager characterManager;

    // Start is called before the first frame update
    void Start()
    {
        diceRoller = new DiceRoller();
        initiativeTracker = new Dictionary<Character, int>();
        actedCharacters = new HashSet<Character>();
        characterManager = FindObjectOfType<CharacterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(nextCharacterToAct))
        {
            //Check which character is to move next
            var nextCharacter = GetCharacterWhoActsNext();

            if (nextCharacter == null)
            {
                return;
            }

            //Set the player turn depending on owner
            nextCharacterToAct = nextCharacter.Name;
            playersTurn = nextCharacter.PlayerControlled;
        }

    }

    internal void SetInitiative(List<Character> characters)
    {
        foreach (var character in characters)
        {
            var d20 = diceRoller.RollDie(20);
            initiativeTracker.Add(character, d20 + character.DEX);
        }
    }

    internal Character GetCharacterWhoActsNext()
    {
        if (string.IsNullOrWhiteSpace(nextCharacterToAct))
        {
            int highestSoFar = 0;
            Character highestCharacter = null;

            foreach (var item in initiativeTracker)
            {
                //have all characters acted? if so reset acted characters
                if (actedCharacters.Count == initiativeTracker.Count)
                {
                    actedCharacters.Clear();
                    actedCharacters = new HashSet<Character>();
                }

                //has the character acted? if so skip
                var actedMatch = actedCharacters.Where(a => a.Name == item.Key.Name).FirstOrDefault();

                if (actedMatch != null)
                {
                    continue;
                }

                if (item.Value > highestSoFar)
                {
                    //current item has higher value, replace highest so far
                    highestSoFar = item.Value;
                    highestCharacter = item.Key;
                }
                else if (highestSoFar == item.Value)
                {
                    if (item.Key.DEX > highestCharacter.DEX)
                    {
                        highestSoFar = item.Value;
                        highestCharacter = item.Key;
                    }
                    else if (highestCharacter.DEX > item.Key.DEX)
                    {
                        //leave as-is
                    }
                    else
                    {
                        //roll for it!
                        var d100 = diceRoller.RollDie(100);
                        if (d100 < 50)
                        {
                            highestSoFar = item.Value;
                            highestCharacter = item.Key;
                        }
                    }
                }
            }

            return highestCharacter;
        }

        //If we get here, return the character who is already assigned to act next
        return characterManager.GetCharacterByName(nextCharacterToAct);
    }

    internal void RemoveCharacterFromInitiative(Character target)
    {
        initiativeTracker.Remove(target);
        if (nextCharacterToAct == target.Name)
        {
            nextCharacterToAct = GetCharacterWhoActsNext().Name;
        }
    }

    internal bool IsItPlayerTurn() => playersTurn;

    internal void ToggleTurn()
    {
        playersTurn = !playersTurn;
    }

    internal void UpdateInitiativeTracker(Character characterJustMoved)
    {
        //Update the initiative tracker to indicate this character has had their go
        nextCharacterToAct = "";
        actedCharacters.Add(characterJustMoved);
    }


    internal void PassTurn(Character c)
    {
        if (c.PlayerControlled)
        {
            print($"{c.Name} passes their turn");
            UpdateInitiativeTracker(c);
        }
    }
}
