using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OfflinePet.Save;
using OfflinePet.Dialogue;
using OfflinePet.Pet;
using OfflinePet.Interaction;
using OfflinePet.Voice;

namespace OfflinePet.UI
{
    /// <summary>
    /// Comfort UI rendered as a World Space canvas positioned in front of the player.
    /// Provides German dialogue subtitles, mood status, activity buttons, and accessibility menus.
    /// </summary>
    public class PetUIManager : MonoBehaviour
    {
        [Header("Dialogue & Status Displays")]
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private TMP_Text petStatusText;
        [SerializeField] private TMP_Text microphoneIndicatorText;

        [Header("Menu Panels")]
        [SerializeField] private GameObject activityPanel;
        [SerializeField] private GameObject voiceFallbackPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject boundaryWarningPanel;

        [Header("Buttons")]
        [SerializeField] private Button feedButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button restButton;
        [SerializeField] private Button petButton;
        [SerializeField] private Button recenterButton;
        [SerializeField] private Button voiceToggleButton;
        [SerializeField] private Button pauseToggleButton;

        private PetController petController;
        private PetInteractionController interactionController;
        private PetVoiceCommandController voiceController;
        private IPetDialogueService dialogueService;

        public void Initialize(
            PetController pet,
            PetInteractionController interaction,
            PetVoiceCommandController voice,
            IPetDialogueService dialogue)
        {
            petController = pet;
            interactionController = interaction;
            voiceController = voice;
            dialogueService = dialogue;

            BindEvents();
            UpdateMoodDisplay();
            ShowSubtitle(dialogueService.GetGreeting(SaveManager.Instance.CurrentData.petName, SaveManager.Instance.CurrentData.personality));
        }

        private void BindEvents()
        {
            if (feedButton != null) feedButton.onClick.AddListener(OnFeedClicked);
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (restButton != null) restButton.onClick.AddListener(OnRestClicked);
            if (petButton != null) petButton.onClick.AddListener(OnPetClicked);
            if (recenterButton != null) recenterButton.onClick.AddListener(OnRecenterClicked);
            if (voiceToggleButton != null) voiceToggleButton.onClick.AddListener(OnVoiceToggleClicked);
            if (pauseToggleButton != null) pauseToggleButton.onClick.AddListener(TogglePause);

            if (interactionController != null)
            {
                interactionController.OnFeedAction += OnFeedClicked;
                interactionController.OnPlayAction += OnPlayClicked;
                interactionController.OnRestAction += OnRestClicked;
                interactionController.OnRecenterAction += OnRecenterClicked;
                interactionController.OnPauseAction += TogglePause;
                interactionController.OnToggleVoiceAction += OnVoiceToggleClicked;
            }

            if (voiceController != null)
            {
                voiceController.OnListeningStateChanged += UpdateMicrophoneVisuals;
                voiceController.OnCommandRecognized += HandleCommandRecognized;
                voiceController.OnStatusMessage += ShowSubtitle;
            }

            if (petController != null && petController.Mood != null)
            {
                petController.Mood.OnStateChanged += UpdateMoodDisplay;
            }
        }

        public void ShowSubtitle(string text)
        {
            if (subtitleText != null)
            {
                subtitleText.text = text;
            }
        }

        public void UpdateMoodDisplay()
        {
            if (petStatusText == null || petController == null || petController.Mood == null) return;

            var mood = petController.Mood;
            string status = $"Freundschaft: Stufe {mood.FriendshipLevel} | " +
                            $"Freude: {Mathf.RoundToInt(mood.Happiness)}% | " +
                            $"Energie: {Mathf.RoundToInt(mood.Energy)}% | " +
                            $"Sättigung: {Mathf.RoundToInt(100f - mood.Hunger)}%";
            petStatusText.text = status;
        }

        private void UpdateMicrophoneVisuals(bool isListening)
        {
            if (microphoneIndicatorText != null)
            {
                microphoneIndicatorText.gameObject.SetActive(isListening);
                microphoneIndicatorText.text = isListening 
                    ? "● Mikrofon aktiv (hört zu...)" 
                    : "";
            }

            if (voiceFallbackPanel != null)
            {
                voiceFallbackPanel.SetActive(isListening);
            }
        }

        private void OnFeedClicked()
        {
            if (petController == null) return;
            string petName = SaveManager.Instance.CurrentData.petName;
            string personality = SaveManager.Instance.CurrentData.personality;

            ShowSubtitle(dialogueService.GetFeedingReaction(petName, "Himmelsbeere", personality));
            petController.PerformFeedAnimation(SaveManager.Instance.Save);
        }

        private void OnPlayClicked()
        {
            if (petController == null) return;
            string petName = SaveManager.Instance.CurrentData.petName;
            string personality = SaveManager.Instance.CurrentData.personality;

            ShowSubtitle(dialogueService.GetPlayReaction(petName, "Lichtball", personality));
            petController.PerformPlayAnimation(SaveManager.Instance.Save);
        }

        private void OnRestClicked()
        {
            if (petController == null) return;
            string petName = SaveManager.Instance.CurrentData.petName;
            string personality = SaveManager.Instance.CurrentData.personality;

            ShowSubtitle(dialogueService.GetRestReaction(petName, personality));
            petController.PerformRestAnimation(SaveManager.Instance.Save);
        }

        private void OnPetClicked()
        {
            if (petController == null) return;
            string petName = SaveManager.Instance.CurrentData.petName;
            string personality = SaveManager.Instance.CurrentData.personality;

            ShowSubtitle(dialogueService.GetPettingReaction(petName, personality));
            petController.PerformPetAnimation(SaveManager.Instance.Save);
        }

        private void OnRecenterClicked()
        {
            petController?.RecenterInFrontOfPlayer();
            ShowSubtitle("Haustier und Menü wurden direkt vor dir neu ausgerichtet.");
        }

        private void OnVoiceToggleClicked()
        {
            if (voiceController == null) return;
            if (voiceController.IsListening)
            {
                voiceController.StopListening();
            }
            else
            {
                voiceController.StartListening();
            }
        }

        private void HandleCommandRecognized(string command)
        {
            string petName = SaveManager.Instance.CurrentData.petName;
            string personality = SaveManager.Instance.CurrentData.personality;

            string reaction = dialogueService.GetVoiceCommandReaction(petName, command, personality);
            ShowSubtitle(reaction);

            switch (command.ToLowerInvariant())
            {
                case "füttern":
                    OnFeedClicked();
                    break;
                case "spielen":
                    OnPlayClicked();
                    break;
                case "ruh dich aus":
                    OnRestClicked();
                    break;
                case "freu dich":
                case "komm her":
                case "hallo":
                    petController?.PerformPlayAnimation();
                    break;
                case "pause":
                case "menü":
                    TogglePause();
                    break;
            }
        }

        public void TogglePause()
        {
            if (pausePanel != null)
            {
                bool isCurrentlyPaused = pausePanel.activeSelf;
                pausePanel.SetActive(!isCurrentlyPaused);
                Time.timeScale = isCurrentlyPaused ? 1.0f : 0.0f;
            }
        }

        public void ShowBoundaryWarning(bool show)
        {
            if (boundaryWarningPanel != null)
            {
                boundaryWarningPanel.SetActive(show);
            }
        }
    }
}
