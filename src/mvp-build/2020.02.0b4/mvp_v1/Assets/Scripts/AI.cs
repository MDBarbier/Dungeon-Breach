using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private TurnManager turnManager;
    private SelectionManager selectionManager;
    private MovementManager movementManager;
    private CombatManager combatManager;
    private CharacterManager characterManager;
    private int framesBeforeActingOriginalValue;
    private CombatLogHandler combatLogHandler;
    private Pathfinder pathfinder;
    private bool aiProcessing;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] int framesBeforeActing = 60;
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        aiProcessing = false;
        pathfinder = FindObjectOfType<Pathfinder>();
        selectionManager = FindObjectOfType<SelectionManager>();
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
        if (turnManager.GetCharacterWhoActsNext() != null 
            && !turnManager.GetCharacterWhoActsNext().PlayerControlled
            && !aiProcessing)
        {
            if (framesBeforeActing <= 0)
            {                
                bool hasMoved = false;
                aiProcessing = true;

                //Get the character that should be acting
                var characterToAct = turnManager.GetCharacterWhoActsNext();

                if (characterToAct == null)
                {
                    if (debugLogging)
                    {
                        print($"returning from AI.{nameof(this.Update)} because the characterToAct is null");
                    }
                    aiProcessing = false;
                    return;
                }
                else
                {
                    if (debugLogging)
                    {
                        print($"AI class, method {nameof(this.Update)} processing turn for {characterToAct.Name}");
                    }

                    
                }

                var charGo = characterManager.GetCharacterGameObject(characterToAct);
                selectionManager.selectedCharacter = (charGo, characterToAct);

                //Get available attacks
                var attacks = combatManager.GetTargetsForAttack(characterToAct, charGo, characterToAct.Range);
                var attack = attacks.FirstOrDefault();
                if (attack.Value != null)
                {
                    combatManager.PerformAttack(characterToAct, characterManager.GetCharacterByName(attack.Value.name));                    
                }
                else 
                {
                    List<Vector3> path = new List<Vector3>();

                    //Select target
                    try
                    {
                        var targetToApproach = pathfinder.FindNearestEnemy(charGo.transform.localPosition);

                        if (targetToApproach.Item1 == null)
                        {
                            combatLogHandler.CombatLog($"{characterToAct.Name} scratches their head and looks confused...");
                        }
                        else
                        {
                            if (debugLogging)
                            {
                                combatLogHandler.CombatLog($"{characterToAct.Name} is giving {targetToApproach.Item2.Name} the evils!");
                            }

                            //find path to target
                            path = pathfinder.Pathfind(targetToApproach.Item1.transform.localPosition, charGo.transform.localPosition, false, false);
                            
                            var moveToTake = (characterToAct.MA > path.Count) ? path.Count : characterToAct.MA;
                           
                            movementManager.MoveCharacter((charGo, characterToAct), path[moveToTake-1]);
                            hasMoved = true;
                        }                        

                    }
                    catch (Exception ex)
                    {
                        print(ex.Message);
                    }
                }        
                
                if (!hasMoved)
                {
                    combatLogHandler.CombatLog($"{characterToAct.Name} bides his time");
                }

                //Update initiative
                turnManager.UpdateInitiativeTracker(characterToAct);

                framesBeforeActing = framesBeforeActingOriginalValue;
                aiProcessing = false;
            }
            else
            {
                framesBeforeActing--;
            }
        }
    }
}
