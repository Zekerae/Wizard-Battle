using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace WizardGame
{
    public class SceneTwo : MonoBehaviour
    {
        [Header("UI Input Fields")]
        [Tooltip("Input field for the Red player's name")]
        public TMP_InputField redInputName;

        [Tooltip("Input field for the Blue player's name")]
        public TMP_InputField blueInputName;

        // Static fields persist across scenes to carry player names into the battle arena
        public static string redName;
        public static string blueName;

        private void Update()
        {
            if (redInputName != null)
            {
                redName = redInputName.text;
            }

            if (blueInputName != null)
            {
                blueName = blueInputName.text;
            }
        }

        // Loads Scene3 (Build Index 2)
        public void NextScene()
        {
            SceneManager.LoadScene(2);
        }
    }
}
