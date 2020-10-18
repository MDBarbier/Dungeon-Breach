using Assets.Scripts.Classes;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoPaneHandler : MonoBehaviour
{
    private string infoPaneText;
    private CharacterManager characterManager;
    private SelectionManager selectionManager;
    private Text infoPaneTextElement;

    // Start is called before the first frame update
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        infoPaneTextElement = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        (GameObject selectedGameObject, Character selectedCharacter) = selectionManager.selectedCharacter;

        if (selectedCharacter != null)
        {
            string infoString = selectedCharacter.ToString();
            infoPaneTextElement.text = infoString;
        }
    }
}
