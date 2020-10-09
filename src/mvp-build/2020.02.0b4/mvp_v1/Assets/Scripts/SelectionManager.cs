using Assets.Scripts.Classes;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private ControlManager controlManager;
    private CharacterManager characterManager;
    private CombatManager combatManager;
    private GameObject lastSelected;
    private GameObject selectionRing;
    private MovementManager movementManager;
    private TurnManager turnManager;
    private Dictionary<(int, int), (GameObject, Material)> highlightedTiles;

    public (GameObject, Character) selectedCharacter;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject selectionIndicator;    
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {        
        combatManager = FindObjectOfType<CombatManager>();
        movementManager = FindObjectOfType<MovementManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        controlManager = FindObjectOfType<ControlManager>();
        turnManager = FindObjectOfType<TurnManager>();
        ResetHighlightedTiles();
    }

    // Update is called once per frame
    void Update()
    {
        AddSelectionIfPlayerCharacterNext();

        //If no click detected end here
        if (controlManager.clickDetectedOn == null)
        {
            return;
        }

        //Handle any click events
        switch (controlManager.clickDetectedOn.tag)
        {
            case "Character":

                //todo :refactor: this logic into character manager

                //Is the clicked on character the target of an attack?
                var targetedCharacters = combatManager.GetTargetedCharacters();

                foreach (var c in targetedCharacters)
                {
                    if (c.ch.Name == controlManager.clickDetectedOn.name)
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

                        //Tell turn controller to update initiative for this character
                        turnManager.UpdateInitiativeTracker(c.attacker);
                                                
                        ResetHighlightedTiles();
                        RemoveSelections();
                        break;
                    }
                }

                break;

            case "Floor":

                //Check whether this tile is in the list of highlighted tiles for the currently selected unit
                foreach (var tile in GetHighlightedTiles())
                {
                    if (tile.Key.Item1 == controlManager.clickDetectedOn.transform.position.x && tile.Key.Item2 == controlManager.clickDetectedOn.transform.position.z)
                    {
                        movementManager.MoveCharacter(selectedCharacter, ((tile.Key.Item1, tile.Key.Item2), tile.Value.Item1));
                        break;
                    }
                }

                
                break;

            case "Scenery":
                
                break;

            case "AdminObject":

                break;

            case null:
            default:                
                break;
        }
    }

    private void AddSelectionIfPlayerCharacterNext()
    {
        if (turnManager != null && characterManager != null)
        {
            var nextCharacterToAct = turnManager.GetCharacterWhoActsNext();
            var charGameObject = characterManager.GetCharacterGameObject(nextCharacterToAct);

            if (charGameObject == null)
            {
                return;
            }

            if (nextCharacterToAct.PlayerControlled)
            {
                //If the game object is the same as the one selected already then return
                if (lastSelected == charGameObject)
                {
                    return;
                }

                selectionRing = Instantiate(selectionIndicator, new Vector3(charGameObject.transform.position.x, 0.35f,
                    charGameObject.transform.position.z), Quaternion.identity);
                selectionRing.name = "SelectionRing";
                lastSelected = charGameObject;

                selectedCharacter = (charGameObject, nextCharacterToAct);
            }
        }
    }

    internal void RemoveSelections()
    {
        if (selectionRing != null)
        {
            Destroy(selectionRing);
        }
        
        lastSelected = null;
    }

    internal void ResetHighlightedTiles()
    {
        if (highlightedTiles != null)
        {
            foreach (var tile in GetHighlightedTiles())
            {
                tile.Value.Item1.GetComponent<MeshRenderer>().material = tile.Value.Item2;
            }

            highlightedTiles.Clear();
        }        

        highlightedTiles = new Dictionary<(int, int), (GameObject, Material)>();
    }

    internal Dictionary<(int, int), (GameObject, Material)> GetHighlightedTiles()
    {
        if (highlightedTiles == null)
        {
            return new Dictionary<(int, int), (GameObject, Material)>();
        }
        else
        {
            return highlightedTiles;
        }
    }

    internal void AddhighlightedSquare(((int, int), (GameObject, Material)) theHighlightedTile)
    {
        this.highlightedTiles.Add(theHighlightedTile.Item1, theHighlightedTile.Item2);
    }
}
