using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class MainSceneSetup : MonoBehaviour
{
    [SerializeField] private UIDocument mainUIDocument;
    
    [Inject] private ILogService logService;
    
    private void Start()
    {
        // Verify our foundation components
        logService.Log("Main scene initialized with UI Toolkit, Zenject, and UTF foundation");
        
        if (mainUIDocument != null)
        {
            logService.Log("UI Document found and initialized");
        }
        else
        {
            logService.LogError("UI Document not found. Please add a UIDocument component.");
        }
    }
}
