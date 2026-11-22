using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

namespace WizardGame
{
    public class SceneThree : MonoBehaviour
    {
        [Header("UI Text References")]
        [Tooltip("Text displaying the Red player's name")]
        public TMP_Text redGameName;

        [Tooltip("Text displaying the Blue player's name")]
        public TMP_Text blueGameName;

        [Tooltip("Text displaying the Red player's HP")]
        public TMP_Text redHPText;

        [Tooltip("Text displaying the Blue player's HP")]
        public TMP_Text blueHPText;

        [Header("Video Clips - Red Wizard")]
        public VideoClip redStance;
        public VideoClip redAttack;

        [Header("Video Clips - Blue Wizard")]
        public VideoClip blueStance;
        public VideoClip blueAttack;

        [Header("Video Players")]
        public VideoPlayer redVidPlayer;
        public VideoPlayer blueVidPlayer;

        [Header("Health Points")]
        public int redHP = 50;
        public int blueHP = 50;

        private void Start()
        {
            // Populate player names from SceneTwo static fields (with fallback defaults)
            if (redGameName != null)
            {
                redGameName.text = !string.IsNullOrEmpty(SceneTwo.redName) ? SceneTwo.redName : "Red";
            }

            if (blueGameName != null)
            {
                blueGameName.text = !string.IsNullOrEmpty(SceneTwo.blueName) ? SceneTwo.blueName : "Blue";
            }

            // Display initial HP values
            if (redHPText != null)
            {
                redHPText.text = redHP.ToString();
            }

            if (blueHPText != null)
            {
                blueHPText.text = blueHP.ToString();
            }

            // Initialize looping idle stance videos
            if (redVidPlayer != null && redStance != null)
            {
                redVidPlayer.clip = redStance;
                redVidPlayer.isLooping = true;
                redVidPlayer.Play();
            }

            if (blueVidPlayer != null && blueStance != null)
            {
                blueVidPlayer.clip = blueStance;
                blueVidPlayer.isLooping = true;
                blueVidPlayer.Play();
            }
        }

        private void Update()
        {
            // Transition to Scene4 (Game Over screen at Build Index 3) when either player's HP reaches 0
            if (redHP <= 0 || blueHP <= 0)
            {
                SceneManager.LoadScene(3);
            }
        }


        // Triggered by Red Attack button: plays attack clip and applies damage on finish
        public void AttackRed()
        {
            if (redVidPlayer != null && redAttack != null)
            {
                redVidPlayer.loopPointReached -= OnVideoFinish;
                redVidPlayer.loopPointReached += OnVideoFinish;
                redVidPlayer.clip = redAttack;
                redVidPlayer.isLooping = false;
                redVidPlayer.Play();
            }
        }


        // Triggered by Blue Attack button: plays attack clip and applies damage on finish
        public void AttackBlue()
        {
            if (blueVidPlayer != null && blueAttack != null)
            {
                blueVidPlayer.loopPointReached -= OnVideoFinish;
                blueVidPlayer.loopPointReached += OnVideoFinish;
                blueVidPlayer.clip = blueAttack;
                blueVidPlayer.isLooping = false;
                blueVidPlayer.Play();
            }
        }

        private void OnVideoFinish(VideoPlayer vp)
        {
            // Immediately unsubscribe to prevent repeated damage triggers
            vp.loopPointReached -= OnVideoFinish;

            if (vp == redVidPlayer)
            {
                // Red attacked -> subtract 10 HP from Blue
                blueHP -= 10;
                if (blueHPText != null)
                {
                    blueHPText.text = blueHP.ToString();
                }

                // Restore Red's looping idle stance
                if (redStance != null)
                {
                    vp.clip = redStance;
                    vp.isLooping = true;
                    vp.Play();
                }
            }
            else if (vp == blueVidPlayer)
            {
                // Blue attacked -> subtract 10 HP from Red
                redHP -= 10;
                if (redHPText != null)
                {
                    redHPText.text = redHP.ToString();
                }

                // Restore Blue's looping idle stance
                if (blueStance != null)
                {
                    vp.clip = blueStance;
                    vp.isLooping = true;
                    vp.Play();
                }
            }
        }

        private void OnDestroy()
        {
            if (redVidPlayer != null)
            {
                redVidPlayer.loopPointReached -= OnVideoFinish;
            }

            if (blueVidPlayer != null)
            {
                blueVidPlayer.loopPointReached -= OnVideoFinish;
            }
        }
    }
}
