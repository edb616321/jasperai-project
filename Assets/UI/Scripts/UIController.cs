using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    [Inject] private ILogService logService;
    
    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        var button = root.Q<Button>("test-button");
        
        if (button != null)
        {
            button.clicked += OnButtonClicked;
        }
    }
    
    private void OnDisable()
    {
        var root = uiDocument.rootVisualElement;
        var button = root.Q<Button>("test-button");
        
        if (button != null)
        {
            button.clicked -= OnButtonClicked;
        }
    }
    
    private void OnButtonClicked()
    {
        logService.Log("Button clicked!");
        Debug.Log("Button clicked! UI Toolkit is working!");
    }
}
