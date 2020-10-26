using UnityEditor;
using UnityEditor.SceneManagement;

public static class LoadMainScene
{   

    [MenuItem("Tools/Load Main Menu Scene")]
    public static void LoadMainMenuScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Load Test Scene")]
    public static void LoadTestScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Test.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Load Generated Dungeon Scene")]
    public static void LoadDungeonScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/GeneratedDungeon.unity", OpenSceneMode.Single);
    }
}
