using Project.Core.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.UI.Menus.MainMenu.Scripts
{
    public class MainMenuBasicManager : MonoBehaviour
    {
        public string characterCreationSceneName = "Project/Scenes/Menus/CharacterCreation";
        public string gameplaySceneName = "Project/Scenes/OpenAreaTestScene";

        // Method to start a new game, going to character creation
        public void StartNewGame()
        {
            // Optionally clear previous save data
            NewSaveManager.Instance.ClearCurrentSave();

            // Load character creation scene
            SceneManager.LoadScene(characterCreationSceneName);
        }
    }
}
