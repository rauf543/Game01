using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene transitions using synchronous loading.
/// Provides methods to load each of the main scenes in the game.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    // Scene names as constants to avoid string errors
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GUILD_HALL_SCENE = "GuildHall";
    public const string MISSION_MAP_SCENE = "MissionMap";
    public const string COMBAT_SCENE = "Combat";

    /// <summary>
    /// Loads the Main Menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    /// <summary>
    /// Loads the Guild Hall scene.
    /// </summary>
    public void LoadGuildHall()
    {
        SceneManager.LoadScene(GUILD_HALL_SCENE);
    }

    /// <summary>
    /// Loads the Mission Map scene.
    /// </summary>
    public void LoadMissionMap()
    {
        SceneManager.LoadScene(MISSION_MAP_SCENE);
    }

    /// <summary>
    /// Loads the Combat scene.
    /// </summary>
    public void LoadCombat()
    {
        SceneManager.LoadScene(COMBAT_SCENE);
    }

    /// <summary>
    /// Generic method to load any scene by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}