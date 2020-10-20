using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePersistenceEngine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateMeleeNo();
        UpdateRangedNo();
        UpdateHealerNo();
        UpdateEnemyMeleeNo();
        UpdateEnemyRangedNo();
    }

    // Update is called once per frame
    void Update()
    {
       
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
            }
        }
    }

    public void NewGame()
    {
        print("New game clicked");
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        print("Quitting game!");
        Application.Quit();
    }

    public void Cancel()
    {        
        print("Cancel pressed");
        SceneManager.LoadScene(0);
        
    }

    public void Ok()
    {
        SceneManager.LoadScene(2);
        print("Ok pressed");
    }
}
