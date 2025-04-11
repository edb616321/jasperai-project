using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

/// <summary>
/// Sets up the main chat scene and UI Toolkit integration
/// </summary>
public class ChatSceneSetup : MonoBehaviour
{
    [SerializeField] private UIDocument chatUIDocument;
    
    [Inject] private ILogService logService;
    [Inject] private IChatService chatService;
    
    private void Start()
    {
        // Verify UI Document reference
        if (chatUIDocument == null)
        {
            Debug.LogError("ChatSceneSetup requires a UIDocument reference!");
            return;
        }
        
        // Log successful initialization
        logService.Log("Chat scene initialized successfully");
        logService.Log("UI Toolkit + Zenject + UTF foundation ready");
    }
    
    /// <summary>
    /// Utility method to create a complete chat scene from code
    /// </summary>
    [ContextMenu("Setup Complete Chat Scene")]
    public void SetupCompleteChatScene()
    {
        // This method can be called from the editor to set up the entire scene
        Debug.Log("Setting up complete chat scene...");
        
        // 1. Create GameObject for UI Document if it doesn't exist
        GameObject uiDocumentGO = GameObject.Find("ChatUIDocument");
        if (uiDocumentGO == null)
        {
            uiDocumentGO = new GameObject("ChatUIDocument");
            
            // Add UIDocument component
            UIDocument document = uiDocumentGO.AddComponent<UIDocument>();
            
            // Find the ChatUI UXML asset
            VisualTreeAsset uxml = Resources.Load<VisualTreeAsset>("Assets/UI/UXML/ChatUI");
            if (uxml != null)
            {
                document.visualTreeAsset = uxml;
            }
            else
            {
                Debug.LogError("Could not find ChatUI.uxml asset!");
            }
            
            // Add ChatUIController
            ChatUIController controller = uiDocumentGO.AddComponent<ChatUIController>();
            controller.enabled = true;
        }
        
        // 2. Create Zenject Scene Context if it doesn't exist
        GameObject sceneContextGO = GameObject.Find("SceneContext");
        if (sceneContextGO == null)
        {
            sceneContextGO = new GameObject("SceneContext");
            sceneContextGO.AddComponent<Zenject.SceneContext>();
        }
        
        // 3. Create Installers GameObject
        GameObject installersGO = GameObject.Find("Installers");
        if (installersGO == null)
        {
            installersGO = new GameObject("Installers");
            installersGO.AddComponent<ChatInstaller>();
        }
        
        Debug.Log("Chat scene setup complete! Press Play to test the UI.");
    }
}
