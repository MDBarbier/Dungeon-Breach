using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUiClickHandler : MonoBehaviour
{
    private GamePersistenceEngine gamePersistenceEngine;
    private int playerMelee;
    private int playerRanged;
    private int playerHealer;
    private int enemyMelee;
    private int enemyRanged;
    private int roomX;
    private int roomZ;

    // Start is called before the first frame update
    void Start()
    {
        gamePersistenceEngine = FindObjectOfType<GamePersistenceEngine>();    
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyMeleeNo();
        UpdateEnemyRangedNo();
        UpdateHealerNo();
        UpdateMeleeNo();
        UpdateRangedNo();
        UpdateRoomX();
        UpdateRoomZ();
    }

    private void UpdateRoomZ()
    {
        var theNumber = GameObject.Find("RoomZNumber");
        var theSlider = GameObject.Find("RoomZSlider");
        if (theNumber != null && theSlider != null)
        {
            var textElements = theNumber.GetComponent<Text>();
            var slider = theSlider.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                roomZ = (int)slider.value;
            }
        }
    }

    public void UpdateMeleeNo()
    {
        var playerMeleeNo = GameObject.Find("PlayerMeleeNumber");
        var playerMeleeSlider = GameObject.Find("PlayerMeleeSlider");
        if (playerMeleeNo != null && playerMeleeSlider != null)
        {
            var textElements = playerMeleeNo.GetComponent<Text>();
            var slider = playerMeleeSlider.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                playerMelee = (int)slider.value;
            }
        }
    }

    public void UpdateRangedNo()
    {
        var playerNo = GameObject.Find("PlayerRangedNumber");
        var playerSlider = GameObject.Find("PlayerRangedSlider");
        if (playerNo != null && playerSlider != null)
        {
            var textElements = playerNo.GetComponent<Text>();
            var slider = playerSlider.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                playerRanged = (int)slider.value;
            }
        }
    }

    public void UpdateEnemyRangedNo()
    {
        var no = GameObject.Find("EnemyRangedNumber");
        var sliderContainer = GameObject.Find("EnemyRangedSlider");
        if (no != null && sliderContainer != null)
        {
            var textElements = no.GetComponent<Text>();
            var slider = sliderContainer.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                enemyRanged = (int)slider.value;
            }
        }
    }

    public void UpdateEnemyMeleeNo()
    {
        var no = GameObject.Find("EnemyMeleeNumber");
        var sliderContainer = GameObject.Find("EnemyMeleeSlider");
        if (no != null && sliderContainer != null)
        {
            var textElements = no.GetComponent<Text>();
            var slider = sliderContainer.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                enemyMelee = (int)slider.value;
            }
        }
    }

    public void UpdateHealerNo()
    {
        var playerNo = GameObject.Find("PlayerHealerNumber");
        var playerSlider = GameObject.Find("PlayerHealerSlider");
        if (playerNo != null && playerSlider != null)
        {
            var textElements = playerNo.GetComponent<Text>();
            var slider = playerSlider.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                playerHealer = (int)slider.value;
            }
        }
    }

    public void UpdateRoomX()
    {
        var theNumber = GameObject.Find("RoomXNumber");
        var theSlider = GameObject.Find("RoomXSlider");
        if (theNumber != null && theSlider != null)
        {
            var textElements = theNumber.GetComponent<Text>();
            var slider = theSlider.GetComponent<Slider>();

            if (textElements != null && slider != null)
            {
                textElements.text = slider.value.ToString();
                roomX = (int)slider.value;
            }
        }
    }

    public void NewGame()
    {
        print("New game clicked");
        gamePersistenceEngine.NewGame();
    }

    public void QuitGame()
    {
        print("Quitting game!");
        gamePersistenceEngine.QuitGame();
    }

    public void Cancel()
    {
        print("Cancel pressed");
        gamePersistenceEngine.BackToMainMenu();
    }

    public void Ok()
    {
        gamePersistenceEngine.StartBattle(new BattleState()
        {
            PlayerMeleeCharacters = playerMelee,
            PlayerHealerCharacters = playerHealer,
            PlayerRangedCharacters = playerRanged,
            EnemyMeleeCharacters = enemyMelee,
            EnemyRangedCharacters = enemyRanged,
            RoomX = roomX,
            RoomZ = roomZ
        });

        print("Ok pressed");
    }
}
