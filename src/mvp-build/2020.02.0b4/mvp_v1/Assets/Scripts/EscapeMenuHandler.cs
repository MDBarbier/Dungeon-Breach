using UnityEngine;

public class EscapeMenuHandler : MonoBehaviour
{

    private ControlManager controlManager;
    private GamePersistenceEngine gamePersistenceEngine;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject escapeMenu;
    [SerializeField] GameObject escapeMenuPanel;
    [SerializeField] GameObject helpMenuPanel;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        controlManager = FindObjectOfType<ControlManager>();
        gamePersistenceEngine = FindObjectOfType<GamePersistenceEngine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlManager.GetEscapeDetected())
        {
            switch (gamePersistenceEngine.GameState)
            {
                case GameState.Setup:
                    break;
                case GameState.Underway:
                    gamePersistenceEngine.GameState = GameState.IngameMenu;
                    controlManager.ResetEscapeDetected();
                    escapeMenu.SetActive(true);
                    escapeMenuPanel.SetActive(true);
                    break;
                case GameState.FinishedWon:
                    break;
                case GameState.FinishedLost:
                    break;
                case GameState.Over:
                    break;
                case GameState.IngameMenu:
                    gamePersistenceEngine.GameState = GameState.Underway;
                    controlManager.ResetEscapeDetected();
                    escapeMenu.SetActive(false);
                    escapeMenuPanel.SetActive(false);
                    break;
                default:
                    break;
            }           
        }
    }
}
