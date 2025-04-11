using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

/// <summary>
/// Simple editor utility for creating a chat scene
/// </summary>
public class ChatSceneBuilder : EditorWindow
{
    [MenuItem("JasperAI/Create Chat Scene")]
    public static void CreateChatScene()
    {
        // Create a new scene
        EditorUtility.DisplayProgressBar("Creating Chat Scene", "Setting up new scene...", 0.1f);
        
        // Create necessary directories
        string scenesDir = Path.Combine(Application.dataPath, "Scenes");
        if (!Directory.Exists(scenesDir))
        {
            Directory.CreateDirectory(scenesDir);
            AssetDatabase.Refresh();
        }
        
        // Save current scene if needed
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorUtility.ClearProgressBar();
            return;
        }
        
        // Create new empty scene
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorUtility.DisplayProgressBar("Creating Chat Scene", "Creating UI elements...", 0.3f);
        
        // Create UI Document GameObject
        GameObject uiDocumentGO = new GameObject("ChatUIDocument");
        UIDocument uiDocument = uiDocumentGO.AddComponent<UIDocument>();
        
        // Set up UI Document with UXML and USS
        string uxmlPath = "Assets/UI/UXML/ChatUI.uxml";
        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        
        if (uxml == null)
        {
            Debug.LogError($"Could not find ChatUI.uxml at {uxmlPath}");
            EditorUtility.ClearProgressBar();
            return;
        }
        
        uiDocument.visualTreeAsset = uxml;
        
        // Add StyleSheet
        string ussPath = "Assets/UI/USS/ChatStyles.uss";
        StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
        
        if (uss != null)
        {
            uiDocument.styleSheets.Add(uss);
        }
        else
        {
            Debug.LogWarning($"Could not find ChatStyles.uss at {ussPath}");
        }
        
        EditorUtility.DisplayProgressBar("Creating Chat Scene", "Adding controller components...", 0.6f);
        
        // Add ChatUIController component
        ChatUIController chatController = uiDocumentGO.AddComponent<ChatUIController>();
        chatController.enabled = true;
        
        // Set up references
        SerializedObject serializedController = new SerializedObject(chatController);
        SerializedProperty uiDocumentProperty = serializedController.FindProperty("uiDocument");
        if (uiDocumentProperty != null)
        {
            uiDocumentProperty.objectReferenceValue = uiDocument;
            serializedController.ApplyModifiedProperties();
        }
        
        EditorUtility.DisplayProgressBar("Creating Chat Scene", "Saving scene...", 0.9f);
        
        // Save the scene
        string scenePath = Path.Combine("Assets/Scenes", "ChatScene.unity");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"Chat scene created at {scenePath}. Press Play to test the UI.");
    }
}
