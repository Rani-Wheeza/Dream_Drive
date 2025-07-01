using UnityEngine;
using DreamDrive.DreamProcessing;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

namespace DreamDrive.SceneGeneration
{
    /// <summary>
    /// Handles the dynamic generation of Unity scenes based on parsed dream data
    /// </summary>
    public class SceneGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class TerrainSettings
        {
            public string settingType;
            public GameObject terrainPrefab;
            public Material skyboxMaterial;
        }

        [System.Serializable]
        public class CharacterSettings
        {
            public string characterType;
            public GameObject characterPrefab;
            public bool isHostile;
        }

        [Header("Environment Settings")]
        public TerrainSettings[] terrainSettings;
        public CharacterSettings[] characterSettings;
        public GameObject[] objectPrefabs;
        public float terrainSpacing = 100f;

        [Header("Atmosphere")]
        public Light mainLight;
        public GameObject fogSystem;
        public PostProcessVolume postProcessing;
        public AudioSource ambientAudioSource;
        public AudioClip[] moodBasedAmbience;

        [Header("Game Logic")]
        public GameObject playerPrefab;
        public float playerSpawnHeight = 2f;
        public LayerMask groundLayer;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        public void GenerateScene(DreamParser.DreamData dreamData)
        {
            ClearCurrentScene();
            SetAtmosphere(dreamData.mood);
            GenerateTerrain(dreamData.setting);
            PlaceCharacters(dreamData.characters);
            PlaceObjects(dreamData.objects);
            SetupGameLogic(dreamData.gameLogic);
            SpawnPlayer();

            Debug.Log($"Scene generated from dream: {dreamData.rawDreamText}");
        }

        private void ClearCurrentScene()
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            spawnedObjects.Clear();
        }

        private void SetAtmosphere(string mood)
        {
            if (mainLight == null || postProcessing == null) return;

            // Base atmosphere settings
            var profile = postProcessing.profile;
            
            switch (mood.ToLower())
            {
                case "mysterious":
                    mainLight.intensity = 0.5f;
                    mainLight.color = new Color(0.7f, 0.7f, 1f);
                    if (fogSystem != null)
                    {
                        fogSystem.SetActive(true);
                        RenderSettings.fogDensity = 0.02f;
                        RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.7f);
                    }
                    PlayAmbientSound(0); // mysterious ambience index
                    break;

                case "peaceful":
                    mainLight.intensity = 1.2f;
                    mainLight.color = new Color(1f, 0.95f, 0.8f);
                    if (fogSystem != null)
                    {
                        fogSystem.SetActive(true);
                        RenderSettings.fogDensity = 0.01f;
                        RenderSettings.fogColor = new Color(0.9f, 0.9f, 1f);
                    }
                    PlayAmbientSound(1); // peaceful ambience index
                    break;

                case "scary":
                    mainLight.intensity = 0.3f;
                    mainLight.color = new Color(1f, 0.6f, 0.6f);
                    if (fogSystem != null)
                    {
                        fogSystem.SetActive(true);
                        RenderSettings.fogDensity = 0.05f;
                        RenderSettings.fogColor = new Color(0.2f, 0.2f, 0.2f);
                    }
                    PlayAmbientSound(2); // scary ambience index
                    break;

                default:
                    mainLight.intensity = 1f;
                    mainLight.color = Color.white;
                    if (fogSystem != null)
                        fogSystem.SetActive(false);
                    break;
            }
        }

        private void GenerateTerrain(string setting)
        {
            TerrainSettings matchingTerrain = System.Array.Find(terrainSettings, 
                t => t.settingType.ToLower().Contains(setting.ToLower()));

            if (matchingTerrain != null && matchingTerrain.terrainPrefab != null)
            {
                GameObject terrain = Instantiate(matchingTerrain.terrainPrefab, Vector3.zero, Quaternion.identity);
                spawnedObjects.Add(terrain);

                if (matchingTerrain.skyboxMaterial != null)
                    RenderSettings.skybox = matchingTerrain.skyboxMaterial;
            }
            else
            {
                Debug.LogWarning($"No matching terrain found for setting: {setting}");
            }
        }

        private void PlaceCharacters(string[] characters)
        {
            foreach (string character in characters)
            {
                CharacterSettings matchingCharacter = System.Array.Find(characterSettings,
                    c => c.characterType.ToLower().Contains(character.ToLower()));

                if (matchingCharacter != null && matchingCharacter.characterPrefab != null)
                {
                    // Find a random position on the terrain
                    Vector3 randomPos = GetRandomPositionOnTerrain();
                    GameObject spawnedChar = Instantiate(matchingCharacter.characterPrefab, 
                        randomPos, Quaternion.identity);
                    
                    spawnedObjects.Add(spawnedChar);

                    // Setup AI behavior based on hostility
                    if (matchingCharacter.isHostile)
                    {
                        // TODO: Add hostile AI behavior component
                    }
                }
            }
        }

        private void PlaceObjects(string[] objects)
        {
            foreach (string obj in objects)
            {
                // Simple random object placement for now
                if (objectPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, objectPrefabs.Length);
                    Vector3 randomPos = GetRandomPositionOnTerrain();
                    GameObject spawnedObj = Instantiate(objectPrefabs[randomIndex], 
                        randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                    
                    spawnedObjects.Add(spawnedObj);
                }
            }
        }

        private void SetupGameLogic(string gameLogic)
        {
            switch (gameLogic.ToLower())
            {
                case "explore":
                    // Set up exploration objectives
                    SetupExplorationMode();
                    break;
                case "collect":
                    // Set up collection objectives
                    SetupCollectionMode();
                    break;
                case "chase":
                    // Set up chase/escape scenario
                    SetupChaseMode();
                    break;
                default:
                    SetupExplorationMode();
                    break;
            }
        }

        private void SpawnPlayer()
        {
            if (playerPrefab != null)
            {
                Vector3 spawnPos = GetRandomPositionOnTerrain();
                spawnPos.y += playerSpawnHeight;
                GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                spawnedObjects.Add(player);
            }
        }

        private Vector3 GetRandomPositionOnTerrain()
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-terrainSpacing/2, terrainSpacing/2),
                1000f,
                Random.Range(-terrainSpacing/2, terrainSpacing/2)
            );

            RaycastHit hit;
            if (Physics.Raycast(randomPos, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                return hit.point;
            }

            return new Vector3(0, 0, 0); // Fallback position
        }

        private void PlayAmbientSound(int moodIndex)
        {
            if (ambientAudioSource != null && moodBasedAmbience != null && 
                moodIndex < moodBasedAmbience.Length && moodBasedAmbience[moodIndex] != null)
            {
                ambientAudioSource.clip = moodBasedAmbience[moodIndex];
                ambientAudioSource.Play();
            }
        }

        private void SetupExplorationMode()
        {
            // TODO: Implement exploration objectives
            Debug.Log("Exploration mode activated");
        }

        private void SetupCollectionMode()
        {
            // TODO: Implement collection objectives
            Debug.Log("Collection mode activated");
        }

        private void SetupChaseMode()
        {
            // TODO: Implement chase scenario
            Debug.Log("Chase mode activated");
        }
    }
} 