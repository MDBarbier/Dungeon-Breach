using Assets.Scripts.Classes;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HoveredInfoHandler : MonoBehaviour
{
    private Text hoverPaneElement;
    private ControlManager controlManager;
    private CharacterManager characterManager;
    private SelectionManager selectionManager;

    // Start is called before the first frame update
    void Start()
    {
        selectionManager = FindObjectOfType<SelectionManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        controlManager = FindObjectOfType<ControlManager>();
        hoverPaneElement = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlManager.hoverDetectedOn != null)
        {
            string textToDisplay;

            switch (controlManager.hoverDetectedOn.tag)
            {
                case "Character":
                    var c = characterManager.GetCharacterByName(controlManager.hoverDetectedOn.name);
                    
                    if (c != selectionManager.selectedCharacter.Item2)
                    {
                        textToDisplay = c.ToString();
                    }
                    else
                    {
                        textToDisplay = string.Empty;
                    }                   

                    break;

                case "Scenery":
                    
                    textToDisplay = PrepareInfoString(new string[] { "This is an impassible obstacle" });
                    
                    break;

                case "Floor":
                default:
                    
                    textToDisplay = string.Empty;
                    
                    break;
            }

            UpdateHoverText(textToDisplay);
        }
        else
        {
            UpdateHoverText(null);
        }
    }

    private void UpdateHoverText(string textToDisplay)
    {
        if (hoverPaneElement != null)
        {
            hoverPaneElement.text = textToDisplay;
        }
    }

    private string PrepareInfoString(string[] message)
    {
        StringBuilder sb = new StringBuilder();

        for (var i = 0; i < message.Length; i++)
        {
            sb.Append($"{message[i]}\n"); 
        }
        
        return sb.ToString();
    }
}
