using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private CombatManager combatManager;    
    private TurnManager turnManager;
    private SelectionManager selectionManager;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();        
        turnManager = FindObjectOfType<TurnManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

                combatManager.ClearTargets();                
                turnManager.UpdateInitiativeTracker(c.attacker);
                selectionManager.ResetHighlightedTiles();
                selectionManager.RemoveSelections();
                break;
                
            }
        }
    }
}
