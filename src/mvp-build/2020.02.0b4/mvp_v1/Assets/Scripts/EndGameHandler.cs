using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameHandler : MonoBehaviour
{
    private GamePersistenceEngine gamePersistenceEngine;
    private GameObject outcomeTextObject;
    
    // Start is called before the first frame update
    void Start()
    {
        gamePersistenceEngine = FindObjectOfType<GamePersistenceEngine>();
        outcomeTextObject = GameObject.Find("OutcomeText");
    }

    // Update is called once per frame
    void Update()
    {
        switch (gamePersistenceEngine.GameState)
        {
            case GameState.FinishedWon:                
                if (outcomeTextObject != null)
                {
                    var t = outcomeTextObject.GetComponent<Text>();
                    t.text = "You emerged victorious!";
                }
                break;
            case GameState.FinishedLost:                
                if (outcomeTextObject != null)
                {
                    var t = outcomeTextObject.GetComponent<Text>();
                    t.text = "Defeat stains your name...";
                }
                break;
            default:
                break;
        }
    }
}
