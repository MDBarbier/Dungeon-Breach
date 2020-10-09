using Assets.Scripts.Classes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private CombatManager combatManager;    
    private TurnManager turnManager;
    private ControlManager controlManager;
    private SelectionManager selectionManager;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();        
        turnManager = FindObjectOfType<TurnManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        controlManager = FindObjectOfType<ControlManager>();
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
                //Attack!
                var result = combatManager.AttackCharacter(c.attacker, c.ch);
                if (result.Item1)
                {
                    print($"{c.attacker.Name} strikes {c.ch.Name} with a mighty blow dealing {result.Item2} damage!");
                }
                else
                {
                    print($"{c.attacker.Name} misses {c.ch.Name}");
                }

                EndOfTurnAdmin(c.attacker);
                break;

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
