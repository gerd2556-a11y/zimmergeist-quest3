using System;
using System.IO;
using UnityEngine;

namespace OfflinePet.Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SaveFileName = "zimmergeist_pet_save.json";
        private string saveFilePath;

        public PetSaveData CurrentData { get; private set; }

        public event Action<PetSaveData> OnDataLoaded;
        public event Action OnDataSaved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            Load();
        }

        public void Save()
        {
            try
            {
                if (CurrentData == null)
                {
                    CurrentData = PetSaveData.CreateDefault();
                }

                CurrentData.lastSavedTimestampUtc = DateTime.UtcNow.ToString("o");
                string json = JsonUtility.ToJson(CurrentData, true);
                File.WriteAllText(saveFilePath, json);
                OnDataSaved?.Invoke();
                Debug.Log($"[SaveManager] Saved successfully to: {saveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to save pet data: {ex.Message}");
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    CurrentData = JsonUtility.FromJson<PetSaveData>(json);

                    if (CurrentData == null)
                    {
                        Debug.LogWarning("[SaveManager] Save file was corrupted. Reverting to safe defaults.");
                        CurrentData = PetSaveData.CreateDefault();
                    }
                    else
                    {
                        ValidateLoadedData(CurrentData);
                    }
                }
                else
                {
                    Debug.Log("[SaveManager] No existing save found. Creating default data.");
                    CurrentData = PetSaveData.CreateDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Error loading save data: {ex.Message}. Loading default state.");
                CurrentData = PetSaveData.CreateDefault();
            }

            OnDataLoaded?.Invoke(CurrentData);
        }

        public void ResetSave()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveManager] Could not delete old save file: {ex.Message}");
            }

            CurrentData = PetSaveData.CreateDefault();
            Save();
            OnDataLoaded?.Invoke(CurrentData);
            Debug.Log("[SaveManager] Save data has been reset to defaults.");
        }

        private void ValidateLoadedData(PetSaveData data)
        {
            if (string.IsNullOrWhiteSpace(data.petName)) data.petName = "Lumi";
            data.trust = Mathf.Clamp(data.trust, 0f, 100f);
            data.happiness = Mathf.Clamp(data.happiness, 0f, 100f);
            data.energy = Mathf.Clamp(data.energy, 0f, 100f);
            data.hunger = Mathf.Clamp(data.hunger, 0f, 100f);
            data.friendshipLevel = Mathf.Clamp(data.friendshipLevel, 1, 10);
            data.textSizeMultiplier = Mathf.Clamp(data.textSizeMultiplier, 0.75f, 2.0f);
            data.masterVolume = Mathf.Clamp01(data.masterVolume);
            data.voiceVolume = Mathf.Clamp01(data.voiceVolume);
            data.sfxVolume = Mathf.Clamp01(data.sfxVolume);

            if (string.IsNullOrEmpty(data.personality)) data.personality = "Curious";
        }
    }
}
