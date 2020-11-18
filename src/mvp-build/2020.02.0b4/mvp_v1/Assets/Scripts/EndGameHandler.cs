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

        if (outcomeTextObject != null)
        {
            var t = outcomeTextObject.GetComponent<Text>();
            t.text = gamePersistenceEngine.GetEndGameMessage();
        }
    }
}
