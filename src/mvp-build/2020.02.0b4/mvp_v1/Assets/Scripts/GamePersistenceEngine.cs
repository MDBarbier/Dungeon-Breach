using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePersistenceEngine : MonoBehaviour
{
    public GameState GameState { get; set; } = GameState.Setup;    
    private BattleState _battleState;
    private AudioSource audioSource;

    public BattleState BattleState { 
        get 
        { 
            if (_battleState == null)
            {
                return new BattleState();
            }
            else
            {
                return _battleState;
            }            
        } 
        private set 
        {
            this._battleState = value;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(FadeAudioSource.StartFade(audioSource, 5.0f, 0.25f));
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleState.PlayerCharacters == 0 && GameState == GameState.Underway)
        {
            GameState = GameState.FinishedLost;
        }

        if (BattleState.EnemyCharacters == 0 && GameState == GameState.Underway)
        {
            GameState = GameState.FinishedWon;
        }

        switch (this.GameState)
        {
            case GameState.Setup:
                break;
            case GameState.Underway:
                break;
            case GameState.FinishedWon:
            case GameState.FinishedLost:
                if (SceneManager.GetActiveScene().buildIndex != 3)
                {
                    SceneManager.LoadScene(3);
                }
                break;
            default:
                break;
        }
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (FindObjectsOfType<GamePersistenceEngine>().Length > 1)
        {
            Destroy(gameObject); //suicide if not the only instance
        }
        else
        {
            DontDestroyOnLoad(gameObject); //persist across scene loads
        }
    }

    public void StartBattle(BattleState battleState)
    {
        this.BattleState = battleState;
        this.GameState = GameState.Underway;
        this.BattleState.EnemyCharacters = battleState.EnemyMeleeCharacters + battleState.EnemyRangedCharacters;
        this.BattleState.PlayerCharacters = battleState.PlayerMeleeCharacters + battleState.PlayerRangedCharacters + battleState.PlayerHealerCharacters;
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void AdjustScore(int amount)
    {
        this.BattleState.Score += amount;
    }
    
    internal void BackToMainMenu()
    {
        GameState = GameState.Setup;
        SceneManager.LoadScene(0);
    }

    internal void AlterPlayerCharacterCount(int v)
    {
        BattleState.PlayerCharacters += v;        
    }
    internal void AlterEnemyCharacterCount(int v)
    {
        BattleState.PlayerCharacters += v;
    }
}

public class BattleState
{
    public int PlayerMeleeCharacters { get; set; }
    public int PlayerRangedCharacters { get; set; }
    public int PlayerHealerCharacters { get; set; }
    public int EnemyMeleeCharacters { get; set; }
    public int EnemyRangedCharacters { get; set; }
    public int PlayerCharacters { get; set; }
    public int EnemyCharacters { get; set; }
    public int RoomX { get; set; }
    public int RoomZ { get; set; }
    public int Score { get; set; }
}

public enum GameState
{
    Setup, Underway, FinishedWon, FinishedLost
}

public static class FadeAudioSource
{
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {        
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
