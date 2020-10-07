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
#pragma warning restore 649

    private DiceRoller diceRoller;
    private Dictionary<Character, int> initiativeTracker;

    // Start is called before the first frame update
    void Start()    
    {
        diceRoller = new DiceRoller();
        initiativeTracker = new Dictionary<Character, int>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check which character is to move next
        var nextCharacter = GetCharacterWhoActsNext();

        if (nextCharacter == null)
        { 
            return; 
        }

        //Set the player turn depending on owner
        playersTurn = nextCharacter.PlayerControlled;
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
        int highestSoFar = 0;
        Character highestCharacter = null;

        foreach (var item in initiativeTracker)
        {
            if (highestSoFar > item.Value)
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

    internal bool IsItPlayerTurn() => playersTurn;

    internal void ToggleTurn()
    {
        playersTurn = !playersTurn;
    }

}
