using System.Collections;
using UnityEngine;
using OfflinePet.Save;
using OfflinePet.Dialogue;

namespace OfflinePet.Pet
{
    /// <summary>
    /// Represents the procedural 3D fantasy creature "Lumi":
    /// A friendly celestial creature made of a glowing core body, two animated eyes, 
    /// soft antenna tufts, and floating aura particles. Safe, calm, predictable motion.
    /// </summary>
    public class PetController : MonoBehaviour
    {
        [Header("Procedural Visuals")]
        [SerializeField] private Transform bodyRoot;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Renderer accentRenderer;
        [SerializeField] private Transform leftEye;
        [SerializeField] private Transform rightEye;

        [Header("Motion & Comfort Parameters")]
        [SerializeField] private float floatAmplitude = 0.04f;
        [SerializeField] private float floatFrequency = 1.2f;
        [SerializeField] private float smoothTurnSpeed = 3.5f;

        public PetState CurrentState { get; private set; } = PetState.IdleFloating;
        public PetMoodSystem Mood { get; private set; }

        private Vector3 targetLocalAnchor = new Vector3(0f, 0.95f, 1.1f);
        private Vector3 currentVelocity;
        private Transform playerCameraTransform;
        private Coroutine activeAnimationRoutine;

        public void Initialize(PetSaveData saveData, Transform cameraTransform)
        {
            playerCameraTransform = cameraTransform;
            Mood = new PetMoodSystem(saveData.trust, saveData.happiness, saveData.energy, saveData.hunger, saveData.friendshipLevel);

            ApplyCustomization(saveData.primaryColor, saveData.accentColor);
            SetComfortMode(saveData.isStandingMode);
        }

        public void ApplyCustomization(Color primary, Color accent)
        {
            if (bodyRenderer != null && bodyRenderer.material != null)
            {
                bodyRenderer.material.color = primary;
            }
            if (accentRenderer != null && accentRenderer.material != null)
            {
                accentRenderer.material.color = accent;
            }
        }

        public void SetComfortMode(bool isStanding)
        {
            // Standing mode places pet around chest/eye level (~1.35m); Seated mode around (~0.95m)
            targetLocalAnchor = isStanding ? new Vector3(0f, 1.35f, 1.25f) : new Vector3(0f, 0.95f, 1.1f);
        }

        public void RecenterInFrontOfPlayer()
        {
            if (playerCameraTransform == null) return;

            Vector3 forwardFlat = Vector3.ProjectOnPlane(playerCameraTransform.forward, Vector3.up).normalized;
            if (forwardFlat == Vector3.zero) forwardFlat = Vector3.forward;

            Vector3 targetWorldPos = playerCameraTransform.position 
                + (forwardFlat * targetLocalAnchor.z) 
                + (Vector3.up * (targetLocalAnchor.y - 0.5f));

            transform.position = targetWorldPos;
            transform.rotation = Quaternion.LookRotation(-forwardFlat, Vector3.up);
            Debug.Log("[PetController] Recaptured pet position safely in front of player.");
        }

        private void Update()
        {
            HandleGentleFloating();
            HandleFacePlayer();
        }

        private void HandleGentleFloating()
        {
            if (bodyRoot == null) return;

            float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            bodyRoot.localPosition = new Vector3(0f, yOffset, 0f);
        }

        private void HandleFacePlayer()
        {
            if (playerCameraTransform == null) return;

            Vector3 lookDirection = playerCameraTransform.position - transform.position;
            lookDirection.y = 0f; // Only rotate along Y for calm, predictable behavior

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothTurnSpeed);
            }
        }

        public void PerformFeedAnimation(Action onComplete = null)
        {
            SwitchRoutine(FeedRoutine(onComplete));
        }

        public void PerformPlayAnimation(Action onComplete = null)
        {
            SwitchRoutine(PlayRoutine(onComplete));
        }

        public void PerformPetAnimation(Action onComplete = null)
        {
            SwitchRoutine(PetRoutine(onComplete));
        }

        public void PerformRestAnimation(Action onComplete = null)
        {
            SwitchRoutine(RestRoutine(onComplete));
        }

        private void SwitchRoutine(IEnumerator routine)
        {
            if (activeAnimationRoutine != null)
            {
                StopCoroutine(activeAnimationRoutine);
            }
            activeAnimationRoutine = StartCoroutine(routine);
        }

        private IEnumerator FeedRoutine(Action onComplete)
        {
            CurrentState = PetState.Eating;
            Mood.Feed();

            // Calm, slow head bob and tilt to simulate nibbling
            Vector3 origScale = bodyRoot.localScale;
            for (int i = 0; i < 3; i++)
            {
                bodyRoot.localScale = origScale * 1.08f;
                yield return new WaitForSeconds(0.25f);
                bodyRoot.localScale = origScale;
                yield return new WaitForSeconds(0.25f);
            }

            CurrentState = PetState.IdleFloating;
            onComplete?.Invoke();
        }

        private IEnumerator PlayRoutine(Action onComplete)
        {
            CurrentState = PetState.PlayingWithToy;
            Mood.Play();

            // Joyful gentle rotation and subtle lift
            Vector3 startPos = transform.position;
            float elapsed = 0f;
            float duration = 2.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float hop = Mathf.Sin(progress * Mathf.PI) * 0.12f;
                transform.position = startPos + Vector3.up * hop;
                bodyRoot.Rotate(Vector3.up, 180f * Time.deltaTime);
                yield return null;
            }

            transform.position = startPos;
            bodyRoot.localRotation = Quaternion.identity;
            CurrentState = PetState.IdleFloating;
            onComplete?.Invoke();
        }

        private IEnumerator PetRoutine(Action onComplete)
        {
            CurrentState = PetState.BeingPetted;
            Mood.Pet();

            // Gentle sideways sway of comfort
            Vector3 startPos = transform.position;
            float elapsed = 0f;
            float duration = 1.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float sway = Mathf.Sin(elapsed * 4f) * 0.03f;
                bodyRoot.localPosition = new Vector3(sway, 0f, 0f);
                yield return null;
            }

            bodyRoot.localPosition = Vector3.zero;
            CurrentState = PetState.IdleFloating;
            onComplete?.Invoke();
        }

        private IEnumerator RestRoutine(Action onComplete)
        {
            CurrentState = PetState.Resting;
            Mood.Rest();

            // Calm lowering and slow breathing pace
            float origFreq = floatFrequency;
            floatFrequency = 0.5f; // slower breathing
            yield return new WaitForSeconds(3.0f);
            floatFrequency = origFreq;

            CurrentState = PetState.IdleFloating;
            onComplete?.Invoke();
        }
    }
}
