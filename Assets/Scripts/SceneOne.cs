using UnityEngine;
using UnityEngine.SceneManagement;

namespace WizardGame
{
    public class SceneOne : MonoBehaviour
    {
        [Header("Build Configuration")]
        [Tooltip("Build index of the next scene (Scene2 = Index 1)")]
        [SerializeField] private int nextSceneIndex = 1;

        // Loads Scene2 (Build Index 1)
        public void NextScene()
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
