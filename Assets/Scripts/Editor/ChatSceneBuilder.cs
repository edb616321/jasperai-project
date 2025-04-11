using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

/// <summary>
/// Editor utility for quickly setting up a chat scene with all required components
/// </summary>
public class ChatSceneBuilder : EditorWindow
{
    [MenuItem("JasperAI/Chat/Create Chat Scene")]
    public static void CreateChatScene()
    {
        // Create scene directory if it doesn't exist
        string scenesDir = Path.Combine(Application.dataPath, "Scenes");
        if (!Directory.Exists(scenesDir))
        {
            Directory.CreateDirectory(scenesDir);
            AssetDatabase.Refresh();
        }
        
        // Ensure scene is saved first
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;
        
        // Create new scene
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        
        // Set up UI Document GameObject
        GameObject uiDocumentGO = new GameObject("ChatUIDocument");
        UIDocument uiDocument = uiDocumentGO.AddComponent<UIDocument>();
        
        // Find the UXML asset
        string uxmlPath = "Assets/UI/UXML/ChatUI.uxml";
        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        if (uxml == null)
        {
            Debug.LogError($"Could not find ChatUI.uxml at {uxmlPath}. Make sure the file exists.");
        }
        else
        {
            uiDocument.visualTreeAsset = uxml;
        }
        
        // Add ChatUIController component
        ChatUIController chatController = uiDocumentGO.AddComponent<ChatUIController>();
        chatController.enabled = true;
        
        // Create scene setup
        GameObject sceneSetupGO = new GameObject("ChatSceneSetup");
        ChatSceneSetup sceneSetup = sceneSetupGO.AddComponent<ChatSceneSetup>();
        
        // Create Zenject Scene Context
        GameObject sceneContextGO = new GameObject("SceneContext");
        sceneContextGO.AddComponent<Zenject.SceneContext>();
        
        // Create installers
        GameObject installersGO = new GameObject("Installers");
        installersGO.AddComponent<ChatInstaller>();
        
        // Setup references
        sceneSetup.chatUIDocument = uiDocument;
        
        // Save scene
        string scenePath = Path.Combine("Assets/Scenes", "ChatScene.unity");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
        
        Debug.Log($"Chat scene created at {scenePath}. Press Play to test the UI.");
    }
    
    [MenuItem("JasperAI/Chat/Setup Test Run")]
    public static void SetupTestRun()
    {
        // Check if tests directory exists, create if not
        string playModeTestsDir = Path.Combine(Application.dataPath, "Tests", "PlayMode");
        if (!Directory.Exists(playModeTestsDir))
        {
            Directory.CreateDirectory(playModeTestsDir);
            AssetDatabase.Refresh();
        }
        
        // Create PlayMode test for UI
        string testPath = Path.Combine(playModeTestsDir, "ChatUITests.cs");
        if (!File.Exists(testPath))
        {
            string testContent = @"using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ChatUITests
{
    private UIDocument uiDocument;
    private TextField inputField;
    private Button sendButton;
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load the chat scene
        SceneManager.LoadScene("ChatScene");
        
        // Wait for scene to load
        yield return null;
        yield return null; // Extra frame for stability
        
        // Find the UI Document
        GameObject uiDocumentGO = GameObject.Find("ChatUIDocument");
        Assert.IsNotNull(uiDocumentGO, "ChatUIDocument GameObject not found");
        
        // Get the UIDocument component
        uiDocument = uiDocumentGO.GetComponent<UIDocument>();
        Assert.IsNotNull(uiDocument, "UIDocument component not found");
        
        // Get UI elements
        inputField = uiDocument.rootVisualElement.Q<TextField>("chat-input-field");
        sendButton = uiDocument.rootVisualElement.Q<Button>("send-button");
        
        Assert.IsNotNull(inputField, "Chat input field not found");
        Assert.IsNotNull(sendButton, "Send button not found");
    }
    
    [UnityTest]
    public IEnumerator ChatUI_SendsMessage_WhenButtonClicked()
    {
        // Arrange
        string testMessage = "Hello, this is a test message";
        inputField.value = testMessage;
        
        // Record initial message count
        int initialMessageCount = uiDocument.rootVisualElement.Q("chat-messages").childCount;
        
        // Act - click the send button
        sendButton.clicked?.Invoke();
        
        // Wait for message processing
        yield return new WaitForSeconds(0.5f);
        
        // Assert - check that messages were added (user message + response)
        int newMessageCount = uiDocument.rootVisualElement.Q("chat-messages").childCount;
        Assert.Greater(newMessageCount, initialMessageCount, "New messages should have been added");
        
        // Verify input field is cleared
        Assert.AreEqual(string.Empty, inputField.value, "Input field should be cleared after sending");
    }
}
";

            File.WriteAllText(testPath, testContent);
            AssetDatabase.Refresh();
            
            Debug.Log($"Created UI test at {testPath}");
        }
        
        // Open Test Runner window
        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.TestTools.TestRunner.TestRunnerWindow,UnityEditor.TestRunner"));
        
        Debug.Log("Test setup complete. Open the Test Runner window to run the tests.");
    }
}
