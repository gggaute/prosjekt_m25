#if AR_FOUNDATION_PRESENT
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AR.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets
{
    /// <summary>
    /// Simplified menu manager for directly selecting and placing objects in AR space.
    /// </summary>
    public class ARSampleMenuManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The menu with all the creatable objects.")]
        GameObject m_ObjectMenu;

        /// <summary>
        /// The menu with all the creatable objects.
        /// </summary>
        public GameObject objectMenu
        {
            get => m_ObjectMenu;
            set => m_ObjectMenu = value;
        }

        [SerializeField]
        [Tooltip("The animator for the object creation menu.")]
        Animator m_ObjectMenuAnimator;

        /// <summary>
        /// The animator for the object creation menu.
        /// </summary>
        public Animator objectMenuAnimator
        {
            get => m_ObjectMenuAnimator;
            set => m_ObjectMenuAnimator = value;
        }

        [SerializeField]
        [Tooltip("The object spawner component in charge of spawning new objects.")]
        ObjectSpawner m_ObjectSpawner;

        /// <summary>
        /// The object spawner component in charge of spawning new objects.
        /// </summary>
        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        [Tooltip("Button that closes the object creation menu.")]
        Button m_CancelButton;

        /// <summary>
        /// Button that closes the object creation menu.
        /// </summary>
        public Button cancelButton
        {
            get => m_CancelButton;
            set => m_CancelButton = value;
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

        /// <summary>
        /// Input to use for the screen tap start position.
        /// </summary>
        public XRInputValueReader<Vector2> tapStartPositionInput
        {
            get => m_TapStartPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
        }

        bool m_ShowObjectMenu;

        void OnEnable()
        {
            m_TapStartPositionInput.EnableDirectActionIfModeUsed();
            m_CancelButton.onClick.AddListener(HideMenu);
        }

        void OnDisable()
        {
            m_TapStartPositionInput.DisableDirectActionIfModeUsed();
            m_ShowObjectMenu = false;
            m_CancelButton.onClick.RemoveListener(HideMenu);
        }

        void Update()
        {
            if (m_ShowObjectMenu)
            {
                var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
                if (!isPointerOverUI && m_TapStartPositionInput.TryReadValue(out _))
                {
                    HideMenu();
                }
            }
        }

        /// <summary>
        /// Called when an object is selected from the menu.
        /// </summary>
        /// <param name="objectIndex">Index of the selected object in the spawner's prefab list.</param>
        public void OnObjectSelected(int objectIndex)
        {
            if (m_ObjectSpawner == null)
            {
                Debug.LogWarning("Menu Manager not configured correctly: no Object Spawner set.", this);
                return;
            }

            if (objectIndex < m_ObjectSpawner.objectPrefabs.Count)
            {
                m_ObjectSpawner.spawnOptionIndex = objectIndex;
                HideMenu();
                // Trigger placement mode
                Debug.Log($"Object {objectIndex} selected. Ready for placement.");
            }
            else
            {
                Debug.LogWarning("Object Spawner not configured correctly: object index larger than number of Object Prefabs.", this);
            }
        }

        public void ShowMenu()
        {
            m_ShowObjectMenu = true;
            m_ObjectMenu.SetActive(true);
            if (!m_ObjectMenuAnimator.GetBool("Show"))
            {
                m_ObjectMenuAnimator.SetBool("Show", true);
            }
        }

        /// <summary>
        /// Triggers hide animation for menu.
        /// </summary>
        public void HideMenu()
        {
            Debug.Log("Hiding object menu...");
            m_ObjectMenuAnimator.SetBool("Show", false);
            m_ShowObjectMenu = false;
        }
    }
}
#endif
