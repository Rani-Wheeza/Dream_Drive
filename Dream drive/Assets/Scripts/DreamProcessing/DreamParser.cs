using UnityEngine;
using System.Threading.Tasks;
using System;

namespace DreamDrive.DreamProcessing
{
    /// <summary>
    /// Handles the parsing of dream input using GPT API and converts it into structured game data
    /// </summary>
    public class DreamParser : MonoBehaviour
    {
        [System.Serializable]
        public class DreamData
        {
            public string setting;
            public string mood;
            public string[] characters;
            public string[] objects;
            public string gameLogic;
            public string rawDreamText;
        }

        private const string GPT_PROMPT_TEMPLATE = @"Extract the following elements from this dream description:
- Setting: The main environment or location
- Mood: The emotional tone or atmosphere
- Characters: Any beings or entities present
- Objects: Important items or objects
- Game Logic: The main action or objective (e.g., chase, collect, explore)

Dream: {0}";

        public async Task<DreamData> ParseDreamInput(string dreamText)
        {
            // TODO: Implement OpenAI GPT API integration
            Debug.Log("Dream parsing requested: " + dreamText);
            
            // Placeholder return until API is integrated
            return new DreamData
            {
                rawDreamText = dreamText,
                setting = "placeholder",
                mood = "mysterious",
                characters = new string[] { "placeholder" },
                objects = new string[] { "placeholder" },
                gameLogic = "explore"
            };
        }
    }
} 