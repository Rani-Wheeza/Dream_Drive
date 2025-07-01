using UnityEngine;
using System.Threading.Tasks;
using DreamDrive.DreamProcessing;
using DreamDrive.SceneGeneration;

namespace DreamDrive.Core
{
    /// <summary>
    /// Main controller that orchestrates the dream-to-game conversion process
    /// </summary>
    public class DreamForgeController : MonoBehaviour
    {
        [Header("Components")]
        public DreamParser dreamParser;
        public SceneGenerator sceneGenerator;

        [Header("UI References")]
        public UnityEngine.UI.InputField dreamInputField;
        public UnityEngine.UI.Button generateButton;
        public UnityEngine.UI.Text statusText;

        private void Start()
        {
            if (generateButton != null)
            {
                generateButton.onClick.AddListener(async () => await ProcessDream());
            }
        }

        public async Task ProcessDream()
        {
            if (dreamParser == null || sceneGenerator == null)
            {
                Debug.LogError("Required components not assigned!");
                return;
            }

            string dreamText = dreamInputField != null ? dreamInputField.text : "Test dream input";
            
            try
            {
                // Update UI
                if (statusText != null) statusText.text = "Processing dream...";

                // Parse dream
                var dreamData = await dreamParser.ParseDreamInput(dreamText);

                // Generate scene
                sceneGenerator.GenerateScene(dreamData);

                // Update UI
                if (statusText != null) statusText.text = "Dream world generated!";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing dream: {e.Message}");
                if (statusText != null) statusText.text = "Error generating dream world";
            }
        }
    }
} 