using UnityEngine;
using OfflinePet.Save;
using OfflinePet.Pet;
using OfflinePet.Dialogue;
using OfflinePet.Interaction;
using OfflinePet.Voice;
using OfflinePet.UI;

namespace OfflinePet.Core
{
    /// <summary>
    /// Master entry point initializing the offline Mixed Reality pet experience.
    /// Configures Passthrough, loads saved states, builds the procedural pet if needed,
    /// and anchors all interactions comfortably in front of the player.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Camera mainXRCustomCamera;
        [SerializeField] private PetController petController;
        [SerializeField] private PetUIManager uiManager;
        [SerializeField] private PetInteractionController interactionController;
        [SerializeField] private PetVoiceCommandController voiceController;

        [Header("Pet Construction Fallback")]
        [SerializeField] private GameObject defaultPetPrefab;

        private IPetDialogueService dialogueService;

        private void Start()
        {
            InitializeExperience();
        }

        private void InitializeExperience()
        {
            Debug.Log("[GameBootstrap] Initializing Offline Quest 3 MR Pet Experience...");

            if (mainXRCustomCamera == null)
            {
                mainXRCustomCamera = Camera.main;
            }

            ConfigurePassthroughCamera(mainXRCustomCamera);

            dialogueService = new LocalPetDialogueService();

            if (SaveManager.Instance == null)
            {
                var saveObj = new GameObject("SaveManager");
                saveObj.AddComponent<SaveManager>();
            }

            PetSaveData currentSave = SaveManager.Instance.CurrentData;

            if (petController == null)
            {
                petController = BuildProceduralLumiPet();
            }

            petController.Initialize(currentSave, mainXRCustomCamera.transform);
            petController.RecenterInFrontOfPlayer();

            if (interactionController == null)
            {
                interactionController = gameObject.AddComponent<PetInteractionController>();
            }

            if (voiceController == null)
            {
                voiceController = gameObject.AddComponent<PetVoiceCommandController>();
            }

            if (uiManager != null)
            {
                uiManager.Initialize(petController, interactionController, voiceController, dialogueService);
            }

            Debug.Log("[GameBootstrap] Initialization complete. Safe MR comfort anchor active.");
        }

        /// <summary>
        /// Ensures the XR Camera displays real-world Passthrough on Meta Quest 3
        /// by setting a solid black clear color with an alpha of 0.0.
        /// </summary>
        private void ConfigurePassthroughCamera(Camera cam)
        {
            if (cam == null) return;

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
            Debug.Log("[GameBootstrap] Camera configured for OpenXR Passthrough blending.");
        }

        /// <summary>
        /// Procedurally creates the friendly fantasy creature "Lumi" if no custom FBX/Prefab is loaded.
        /// </summary>
        private PetController BuildProceduralLumiPet()
        {
            GameObject petObject = new GameObject("Pet_Lumi");
            PetController controller = petObject.AddComponent<PetController>();

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Body";
            body.transform.SetParent(petObject.transform, false);
            body.transform.localScale = new Vector3(0.32f, 0.35f, 0.3f);

            // Left Eye
            GameObject eyeL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeL.name = "Eye_L";
            eyeL.transform.SetParent(body.transform, false);
            eyeL.transform.localPosition = new Vector3(-0.25f, 0.2f, 0.42f);
            eyeL.transform.localScale = new Vector3(0.18f, 0.22f, 0.12f);
            eyeL.GetComponent<Renderer>().material.color = Color.black;

            // Right Eye
            GameObject eyeR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeR.name = "Eye_R";
            eyeR.transform.SetParent(body.transform, false);
            eyeR.transform.localPosition = new Vector3(0.25f, 0.2f, 0.42f);
            eyeR.transform.localScale = new Vector3(0.18f, 0.22f, 0.12f);
            eyeR.GetComponent<Renderer>().material.color = Color.black;

            // Antenna Orb (Accent)
            GameObject accentOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            accentOrb.name = "AccentOrb";
            accentOrb.transform.SetParent(body.transform, false);
            accentOrb.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            accentOrb.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            return controller;
        }
    }
}
