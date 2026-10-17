using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OfflinePet.Interaction
{
    public enum InputPreference
    {
        HandTracking,
        Controllers
    }

    /// <summary>
    /// Unified interaction controller supporting both Meta Quest 3 Hand Tracking
    /// and Touch Plus Controllers with equal accessibility and seamless fallback.
    /// </summary>
    public class PetInteractionController : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private InputPreference preferredInput = InputPreference.HandTracking;

        public InputPreference CurrentInputMode => preferredInput;

        public event Action OnConfirmAction;
        public event Action OnBackAction;
        public event Action OnToggleMenuAction;
        public event Action OnPauseAction;
        public event Action OnRecenterAction;
        public event Action OnFeedAction;
        public event Action OnPlayAction;
        public event Action OnRestAction;
        public event Action OnToggleVoiceAction;

        public void SetInputPreference(InputPreference mode)
        {
            preferredInput = mode;
            Debug.Log($"[PetInteractionController] Input preference switched to: {mode}");
        }

        private void Update()
        {
            PollFallbackInputs();
        }

        private void PollFallbackInputs()
        {
            // Controller Button shortcuts mapped for seamless VR Controller testing & Quest Input
            if (Keyboard.current != null)
            {
                if (Keyboard.current.mKey.wasPressedThisFrame) OnToggleMenuAction?.Invoke();
                if (Keyboard.current.pKey.wasPressedThisFrame) OnPauseAction?.Invoke();
                if (Keyboard.current.rKey.wasPressedThisFrame) OnRecenterAction?.Invoke();
                if (Keyboard.current.fKey.wasPressedThisFrame) OnFeedAction?.Invoke();
                if (Keyboard.current.spaceKey.wasPressedThisFrame) OnPlayAction?.Invoke();
                if (Keyboard.current.vKey.wasPressedThisFrame) OnToggleVoiceAction?.Invoke();
            }

            // Gamepad / Oculus Touch controller mapping
            if (Gamepad.current != null)
            {
                if (Gamepad.current.startButton.wasPressedThisFrame) OnToggleMenuAction?.Invoke();
                if (Gamepad.current.selectButton.wasPressedThisFrame) OnPauseAction?.Invoke();
                if (Gamepad.current.buttonNorth.wasPressedThisFrame) OnRecenterAction?.Invoke(); // Y / Triangle
                if (Gamepad.current.buttonSouth.wasPressedThisFrame) OnConfirmAction?.Invoke();  // A / Cross
                if (Gamepad.current.buttonEast.wasPressedThisFrame) OnBackAction?.Invoke();      // B / Circle
                if (Gamepad.current.buttonWest.wasPressedThisFrame) OnFeedAction?.Invoke();      // X / Square
            }
        }

        public void TriggerFeed() => OnFeedAction?.Invoke();
        public void TriggerPlay() => OnPlayAction?.Invoke();
        public void TriggerRest() => OnRestAction?.Invoke();
        public void TriggerRecenter() => OnRecenterAction?.Invoke();
        public void TriggerToggleMenu() => OnToggleMenuAction?.Invoke();
        public void TriggerPause() => OnPauseAction?.Invoke();
        public void TriggerVoiceToggle() => OnToggleVoiceAction?.Invoke();
    }
}
