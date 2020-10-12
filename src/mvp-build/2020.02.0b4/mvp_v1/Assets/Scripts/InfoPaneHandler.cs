using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
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
            string infoString = PrepareInfoString(selectedCharacter);
            infoPaneTextElement.text = infoString;
        }
    }

    private string PrepareInfoString(Character character)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Selected unit details\n");
        sb.Append($"Name: {character.Name}\n");
        sb.Append($"HP: {character.HP}/{character.MAXHP}\n");
        sb.Append($"AC: {character.AC}\n");
        sb.Append($"STR: {character.STR}\n");
        sb.Append($"DEX: {character.DEX}\n");
        return sb.ToString();
    }
}
