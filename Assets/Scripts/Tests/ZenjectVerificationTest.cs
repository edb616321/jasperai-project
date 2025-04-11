using UnityEngine;
using System;

#if ZENJECT_PRESENT
using Zenject;
#endif

/// <summary>
/// Simple script to verify if Zenject is properly installed
/// </summary>
public class ZenjectVerificationTest : MonoBehaviour
{
    void Start()
    {
        // Check if Zenject is available
        VerifyZenjectInstallation();
    }

    public void VerifyZenjectInstallation()
    {
#if ZENJECT_PRESENT
        Debug.Log("<color=green>SUCCESS: Zenject is installed and available!</color>");
        try {
            // Try to create a simple container to verify it's working
            DiContainer container = new DiContainer();
            container.Bind<TestService>().AsSingle();
            var service = container.Resolve<TestService>();
            
            Debug.Log("<color=green>SUCCESS: Zenject container created and binding works!</color>");
            Debug.Log($"<color=green>Test service message: {service.GetMessage()}</color>");
        }
        catch (Exception e) {
            Debug.LogError($"Error testing Zenject container: {e.Message}");
        }
#else
        Debug.LogError("<color=red>ERROR: Zenject is not properly installed or ZENJECT_PRESENT define not set.</color>");
        // Also check if types exist even without the define
        try {
            Type containerType = Type.GetType("Zenject.DiContainer, Zenject");
            if (containerType != null) {
                Debug.Log("<color=yellow>NOTE: Zenject types are available, but ZENJECT_PRESENT define not set.</color>");
            } else {
                Debug.LogError("<color=red>ERROR: Zenject DiContainer type not found. Verify installation.</color>");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error checking for Zenject types: {e.Message}");
        }
#endif
    }
}

/// <summary>
/// Simple test class for Zenject binding
/// </summary>
public class TestService
{
    public string GetMessage()
    {
        return "Zenject is working correctly!";
    }
}
