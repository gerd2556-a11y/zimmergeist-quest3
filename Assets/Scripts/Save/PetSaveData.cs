using System;
using UnityEngine;

namespace OfflinePet.Save
{
    [Serializable]
    public class PetSaveData
    {
        public int saveVersion = 1;

        // Customization
        public string petName = "Lumi";
        public Color primaryColor = new Color(0.45f, 0.75f, 1f, 1f);
        public Color accentColor = new Color(1f, 0.85f, 0.35f, 1f);
        public string personality = "Curious"; // Curious, Calm, Playful

        // Core Pet States (0 to 100)
        public float trust = 50f;
        public float happiness = 60f;
        public float energy = 80f;
        public float hunger = 40f; // 0 = satiated, 100 = very hungry
        public int friendshipLevel = 1;
        public int totalPlaySessions = 0;

        // Settings & Accessibility
        public bool isStandingMode = false;
        public bool useHandTracking = true;
        public bool subtitlesEnabled = true;
        public float textSizeMultiplier = 1.0f;
        public float masterVolume = 0.8f;
        public float voiceVolume = 0.9f;
        public float sfxVolume = 0.8f;
        public bool simplifiedMotion = false;
        public bool reducedVisualEffects = false;
        public bool timeDecayEnabled = false; // Player choice: state changes over real time
        public string lastSavedTimestampUtc = "";

        public static PetSaveData CreateDefault()
        {
            return new PetSaveData
            {
                saveVersion = 1,
                petName = "Lumi",
                primaryColor = new Color(0.45f, 0.75f, 1.0f, 1.0f),
                accentColor = new Color(1.0f, 0.85f, 0.35f, 1.0f),
                personality = "Curious",
                trust = 50f,
                happiness = 65f,
                energy = 85f,
                hunger = 30f,
                friendshipLevel = 1,
                totalPlaySessions = 0,
                isStandingMode = false,
                useHandTracking = true,
                subtitlesEnabled = true,
                textSizeMultiplier = 1.0f,
                masterVolume = 0.8f,
                voiceVolume = 0.9f,
                sfxVolume = 0.8f,
                simplifiedMotion = false,
                reducedVisualEffects = false,
                timeDecayEnabled = false,
                lastSavedTimestampUtc = DateTime.UtcNow.ToString("o")
            };
        }
    }
}
