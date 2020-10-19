using Assets.Scripts.Classes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private CombatManager combatManager;    
    private TurnManager turnManager;
    private ControlManager controlManager;
    private SelectionManager selectionManager;
    private CombatLogHandler combatLogHandler;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();        
        turnManager = FindObjectOfType<TurnManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        controlManager = FindObjectOfType<ControlManager>();
        combatLogHandler = FindObjectOfType<CombatLogHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlManager.GetSpacebarDetected())
        {
            controlManager.ResetSpacebarDetection();
            var c = turnManager.GetCharacterWhoActsNext();
            EndOfTurnAdmin(c);
            turnManager.PassTurn(c);
        }
    }

    internal void HandleCharacterClick(GameObject clickDetectedOn)
    {
        var targetedCharacters = combatManager.GetTargetedCharacters();

        foreach (var c in targetedCharacters)
        {
            if (c.ch.Name == clickDetectedOn.name)
            {
                if (c.ch.PlayerControlled)
                {
                    //heal
                    combatManager.PerformHealing(c.attacker, c.ch);
                    EndOfTurnAdmin(c.attacker);
                    break; 
                }
                else
                {
                    //Attack
                    combatManager.PerformAttack(c.attacker, c.ch);
                    EndOfTurnAdmin(c.attacker);
                    break;
                }

            }
        }

        controlManager.clickDetectedOn = null;
    }

    private void EndOfTurnAdmin(Character c)
    {
        combatManager.ClearTargets();
        turnManager.UpdateInitiativeTracker(c);
        selectionManager.ResetHighlightedTiles();
        selectionManager.RemoveSelections();
    }
}
