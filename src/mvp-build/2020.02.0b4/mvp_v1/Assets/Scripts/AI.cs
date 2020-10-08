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

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] int framesBeforeActing = 60;
    [SerializeField] AIStates aiState;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        aiState = AIStates.Waiting;
        framesBeforeActingOriginalValue = framesBeforeActing;
        turnManager = FindObjectOfType<TurnManager>();
        movementManager = FindObjectOfType<MovementManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        combatManager = FindObjectOfType<CombatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!turnManager.IsItPlayerTurn())
        {
            aiState = AIStates.Thinking;

            if (framesBeforeActing <= 0)
            {
                aiState = AIStates.Acting;                

                //Get the character that should be acting
                var characterToAct = turnManager.GetCharacterWhoActsNext();
                var charGo = characterManager.GetCharacterGameObject(characterToAct);

                //Check it's AI controlled
                if (characterToAct.PlayerControlled)
                {
                    throw new Exception("AI should never be passed a player character!");
                }

                //Get available attacks
                var attacks = combatManager.GetTargetsForAttack(characterToAct, charGo);
                var attack = attacks.FirstOrDefault();
                if (attack.Value != null)
                {
                    var resultOfAttack = combatManager.AttackCharacter(characterToAct, characterManager.GetCharacterByName(attack.Value.name));
                    print($"{characterToAct.Name} attacks {attack.Value.name} and the attack is {resultOfAttack.Item1} for {resultOfAttack.Item2} damage!");
                }
                else 
                {
                    //Get available moves
                    var moves = movementManager.GetMoves((charGo, characterToAct));

                    //Take the first move
                    var move = moves.FirstOrDefault();
                    if (move.Value != null)
                    {
                        movementManager.MoveCharacter((charGo, characterToAct), ((move.Key.Item1, move.Key.Item2), move.Value));
                    }
                }               

                //Update initiative
                turnManager.UpdateInitiativeTracker(characterToAct);

                framesBeforeActing = framesBeforeActingOriginalValue;
                aiState = AIStates.Waiting;
            }
            else
            {
                framesBeforeActing--;
            }
        }
    }
}
