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
    private Dictionary<Character, int> masterInitiativeRecord;
    private List<Character> turnOrderTracker;    
    private CombatLogHandler combatLogHandler;
    private TurnOrderLogHandler turnOrderLogHandler;

    // Start is called before the first frame update
    void Start()
    {
        turnOrderLogHandler = FindObjectOfType<TurnOrderLogHandler>();        
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
        diceRoller = new DiceRoller();
        masterInitiativeRecord = new Dictionary<Character, int>();
        turnOrderTracker = new List<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(nextCharacterToAct))
        {            
            var nextCharacter = GetCharacterWhoActsNext();

            if (nextCharacter == null)
            {
                return;
            }

            nextCharacterToAct = nextCharacter.Name;
            playersTurn = nextCharacter.PlayerControlled;
        }

    }

    internal void SetInitiative(List<Character> characters)
    {
        foreach (var character in characters)
        {
            var d20 = diceRoller.RollDie(20);
            masterInitiativeRecord.Add(character, d20 + character.DEX);
        }

        UpdateTurnOrderTracker();

    }

    private void UpdateTurnOrderTracker()
    {
        turnOrderTracker = new List<Character>();
        var tempList = masterInitiativeRecord.ToList();
        tempList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        
        foreach (var item in tempList)
        {
            turnOrderTracker.Add(item.Key);
        }

        if (turnOrderTracker != null)
        {
            turnOrderLogHandler.TurnLog(turnOrderTracker);
        }
    }    

    internal void RemoveCharacterFromInitiative(Character target)
    {
        masterInitiativeRecord.Remove(target);
        turnOrderTracker.Remove(target);

        if (nextCharacterToAct == target.Name)
        {
            nextCharacterToAct = GetCharacterWhoActsNext().Name;
        }
    }     

    internal void UpdateInitiativeTracker(Character characterJustMoved)
    {        
        nextCharacterToAct = "";        
        var nextChar = turnOrderTracker.Where(a => a.Name == characterJustMoved.Name).Single();
        turnOrderTracker.Remove(nextChar);
        turnOrderTracker.Add(nextChar);
        turnOrderLogHandler.TurnLog(turnOrderTracker);
        nextCharacterToAct = GetCharacterWhoActsNext().Name;        
    }

    internal void PassTurn(Character c)
    {
        if (c.PlayerControlled)
        {
            combatLogHandler.CombatLog($"{c.Name} passes their turn");
            UpdateInitiativeTracker(c);
        }
    }

    internal Character GetCharacterWhoActsNext() => turnOrderTracker.FirstOrDefault();
}
