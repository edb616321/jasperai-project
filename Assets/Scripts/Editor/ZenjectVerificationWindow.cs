using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

/// <summary>
/// Editor window to verify Zenject installation and test basic functionality
/// </summary>
public class ZenjectVerificationWindow : EditorWindow
{
    private bool zenjectDetected = false;
    private bool containerCreated = false;
    private bool bindingWorks = false;
    private string statusMessage = "Click 'Test Zenject Installation' to verify";
    private GUIStyle headerStyle;
    private GUIStyle successStyle;
    private GUIStyle errorStyle;
    private GUIStyle normalStyle;
    
    [MenuItem("JasperAI/Tools/Verify Zenject Installation")]
    public static void ShowWindow()
    {
        var window = GetWindow<ZenjectVerificationWindow>("Zenject Verification");
        window.minSize = new Vector2(400, 250);
        window.Show();
    }
    
    private void OnEnable()
    {
        // Try to detect Zenject assembly
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name.Contains("Zenject"))
                {
                    zenjectDetected = true;
                    statusMessage = "Zenject assembly detected. Click 'Test' to verify functionality.";
                    break;
                }
            }
            
            if (!zenjectDetected)
            {
                statusMessage = "Zenject assembly not detected. Please check your installation.";
            }
        }
        catch (Exception ex)
        {
            statusMessage = $"Error detecting Zenject: {ex.Message}";
        }
    }
    
    private void OnGUI()
    {
        // Initialize styles
        if (headerStyle == null)
        {
            InitStyles();
        }
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Header
        EditorGUILayout.LabelField("Zenject Installation Verification", headerStyle);
        EditorGUILayout.Space();
        
        // Status box
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(statusMessage, zenjectDetected ? successStyle : errorStyle);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Test results
        if (zenjectDetected)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Test Results:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("✓ Zenject assembly detected", successStyle);
            EditorGUILayout.LabelField(containerCreated ? "✓ DiContainer created successfully" : "□ DiContainer creation not tested", 
                containerCreated ? successStyle : normalStyle);
            EditorGUILayout.LabelField(bindingWorks ? "✓ Binding and resolution works" : "□ Binding and resolution not tested", 
                bindingWorks ? successStyle : normalStyle);
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Test button
        if (GUILayout.Button("Test Zenject Installation", GUILayout.Height(30)))
        {
            TestZenjectInstallation();
        }
        
        EditorGUILayout.Space();
        
        // Fix button - only show if problems detected
        if (zenjectDetected && (!containerCreated || !bindingWorks))
        {
            if (GUILayout.Button("Create Test Scene", GUILayout.Height(30)))
            {
                CreateTestScene();
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void InitStyles()
    {
        headerStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
        
        successStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.green }
        };
        
        errorStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.red }
        };
        
        normalStyle = new GUIStyle(EditorStyles.label);
    }
    
    private void TestZenjectInstallation()
    {
        try
        {
            // Check if Zenject types exist using reflection
            Type containerType = Type.GetType("Zenject.DiContainer, Zenject") ?? 
                                Type.GetType("Zenject.DiContainer, Zenject-Usage") ?? 
                                FindTypeInLoadedAssemblies("Zenject.DiContainer");
                                
            if (containerType == null)
            {
                zenjectDetected = false;
                containerCreated = false;
                bindingWorks = false;
                statusMessage = "ERROR: Could not find Zenject.DiContainer type. Check installation.";
                return;
            }
            
            // Create DiContainer instance using reflection
            object container = Activator.CreateInstance(containerType);
            if (container == null)
            {
                statusMessage = "ERROR: Failed to create DiContainer instance.";
                return;
            }
            
            containerCreated = true;
            
            // Try binding a test class
            Type testServiceType = typeof(TestService);
            
            // Find and invoke the Bind<> method
            MethodInfo bindMethod = containerType.GetMethod("Bind", Type.EmptyTypes);
            if (bindMethod == null)
            {
                statusMessage = "ERROR: Could not find Bind method on DiContainer.";
                return;
            }
            
            // Make it Bind<TestService>()
            MethodInfo bindGenericMethod = bindMethod.MakeGenericMethod(testServiceType);
            object bindingObj = bindGenericMethod.Invoke(container, null);
            
            // Find and invoke AsSingle
            Type bindingType = bindingObj.GetType();
            MethodInfo asSingleMethod = bindingType.GetMethod("AsSingle", Type.EmptyTypes);
            if (asSingleMethod == null)
            {
                statusMessage = "ERROR: Could not find AsSingle method.";
                return;
            }
            
            asSingleMethod.Invoke(bindingObj, null);
            
            // Try resolving
            MethodInfo resolveMethod = containerType.GetMethod("Resolve", Type.EmptyTypes);
            if (resolveMethod == null)
            {
                statusMessage = "ERROR: Could not find Resolve method.";
                return;
            }
            
            MethodInfo resolveGenericMethod = resolveMethod.MakeGenericMethod(testServiceType);
            object service = resolveGenericMethod.Invoke(container, null);
            
            if (service == null)
            {
                statusMessage = "ERROR: Failed to resolve TestService.";
                return;
            }
            
            bindingWorks = true;
            statusMessage = "SUCCESS: Zenject is properly installed and working!";
        }
        catch (Exception ex)
        {
            statusMessage = $"ERROR: {ex.Message}";
            Debug.LogError($"Zenject verification error: {ex.Message}\n{ex.StackTrace}");
        }
    }
    
    private Type FindTypeInLoadedAssemblies(string fullTypeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(fullTypeName);
            if (type != null)
                return type;
        }
        return null;
    }
    
    private void CreateTestScene()
    {
        // Check if we need to save current scene
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;
            
        // Create new scene
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        
        // Create test game object
        GameObject testObject = new GameObject("ZenjectTest");
        testObject.AddComponent<ZenjectVerificationTest>();
        
        // Create scene context if available
        try
        {
            Type sceneContextType = FindTypeInLoadedAssemblies("Zenject.SceneContext");
            if (sceneContextType != null)
            {
                GameObject sceneContextObject = new GameObject("SceneContext");
                sceneContextObject.AddComponent(sceneContextType);
                
                Debug.Log("Created test scene with SceneContext. Press Play to test Zenject.");
                statusMessage = "Test scene created. Press Play to test Zenject in the Console.";
            }
            else
            {
                Debug.LogWarning("Could not find Zenject.SceneContext type. Test scene created without SceneContext.");
                statusMessage = "Test scene created without SceneContext. Zenject may not be properly installed.";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error creating test scene: {ex.Message}");
            statusMessage = $"Error creating test scene: {ex.Message}";
        }
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
