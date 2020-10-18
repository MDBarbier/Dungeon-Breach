using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private TurnManager turnManager;
    private MovementManager movementManager;
    private CombatManager combatManager;
    private CharacterManager characterManager;
    private int framesBeforeActingOriginalValue;
    private CombatLogHandler combatLogHandler;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] int framesBeforeActing = 60;
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {        
        framesBeforeActingOriginalValue = framesBeforeActing;
        turnManager = FindObjectOfType<TurnManager>();
        movementManager = FindObjectOfType<MovementManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        combatManager = FindObjectOfType<CombatManager>();
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turnManager.GetCharacterWhoActsNext() != null && !turnManager.GetCharacterWhoActsNext().PlayerControlled)
        {
            if (framesBeforeActing <= 0)
            {                
                bool hasMoved = false;

                //Get the character that should be acting
                var characterToAct = turnManager.GetCharacterWhoActsNext();

                if (characterToAct == null)
                {
                    if (debugLogging)
                    {
                        print($"returning from AI.{nameof(this.Update)} because the characterToAct is null");
                    }

                    return;
                }

                var charGo = characterManager.GetCharacterGameObject(characterToAct);                

                //Get available attacks
                var attacks = combatManager.GetTargetsForAttack(characterToAct, charGo);
                var attack = attacks.FirstOrDefault();
                if (attack.Value != null)
                {
                    combatManager.AttackCharacter(characterToAct, characterManager.GetCharacterByName(attack.Value.name));                    
                }
                else 
                {
                    bool increaseXaxis = false;
                    bool increaseZaxis = false;                    

                    //Select target
                    try
                    {
                        var targetToApproach = combatManager.SelectTarget(characterToAct, charGo);
                        if (debugLogging)
                        {
                            combatLogHandler.CombatLog($"{characterToAct.Name} is giving {targetToApproach.Item2.name} the evils!"); 
                        }

                        //compare the position of the target to current position
                        if (targetToApproach.Item1.Item1 > charGo.transform.position.x)
                        {
                            increaseXaxis = true;
                        }

                        if (targetToApproach.Item1.Item2 > charGo.transform.position.z)
                        {
                            increaseZaxis = true;
                        }                       

                    }
                    catch (Exception ex)
                    {
                        print(ex.Message);
                    }

                    //Get available moves
                    var moves = movementManager.GetMoves((charGo, characterToAct));

                    //Use the calculated axis to select a legal move that's in the right direction
                    foreach (var move in moves)
                    {
                        bool xMatch = false;
                        bool zMatch = false;

                        if (increaseXaxis)
                        {
                            if (move.Key.Item1 >= charGo.transform.position.x)
                            {
                                xMatch = true;
                            } 
                        }
                        else
                        {

                            if (move.Key.Item1 <= charGo.transform.position.x)
                            {
                                xMatch = true;
                            }
                        }

                        if (increaseZaxis)
                        {
                            if (move.Key.Item2 >= charGo.transform.position.z)
                            {
                                zMatch = true;
                            }
                        }
                        else
                        {

                            if (move.Key.Item2 <= charGo.transform.position.z)
                            {
                                zMatch = true;
                            }
                        }

                        if (zMatch && xMatch)
                        {
                            movementManager.MoveCharacter((charGo, characterToAct), ((move.Key.Item1, move.Key.Item2), move.Value));
                            hasMoved = true;
                        }
                    }                    
                }        
                
                if (!hasMoved)
                {
                    combatLogHandler.CombatLog($"{characterToAct.Name} bides his time");
                }

                //Update initiative
                turnManager.UpdateInitiativeTracker(characterToAct);

                framesBeforeActing = framesBeforeActingOriginalValue;                
            }
            else
            {
                framesBeforeActing--;
            }
        }
    }
}
