using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEditor.SceneManagement;

/// <summary>
/// Simple editor utility for creating a complete game scene with player and NPC
/// </summary>
public class GameSceneBuilder : EditorWindow
{
    [MenuItem("JasperAI/Create Game Scene")]
    public static void CreateGameScene()
    {
        // Create a new scene
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Setting up new scene...", 0.1f);
        
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
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Setting up environment...", 0.2f);
        
        // Create a simple ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5, 1, 5);
        
        // Create directional light if needed
        if (GameObject.Find("Directional Light") == null)
        {
            GameObject light = new GameObject("Directional Light");
            Light lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 1f;
            lightComponent.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50, 30, 0);
            light.transform.position = new Vector3(0, 10, 0);
        }
        
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Creating Player...", 0.4f);
        
        // Create player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1, 0);
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerController>();
        
        // Add main camera as child of player
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera == null)
        {
            mainCamera = new GameObject("Main Camera");
            mainCamera.AddComponent<Camera>();
            mainCamera.tag = "MainCamera";
        }
        mainCamera.transform.SetParent(player.transform);
        mainCamera.transform.localPosition = new Vector3(0, 0.7f, 0);
        
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Creating NPC...", 0.6f);
        
        // Create an NPC
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name = "NPC";
        npc.transform.position = new Vector3(3, 1, 3);
        npc.GetComponent<Renderer>().material.color = Color.blue;
        
        // Add NPC script
        NPC npcScript = npc.AddComponent<NPC>();
        
        // Create interaction prompt for NPC
        GameObject prompt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        prompt.name = "InteractionPrompt";
        prompt.transform.SetParent(npc.transform);
        prompt.transform.localPosition = new Vector3(0, 1.5f, 0);
        prompt.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        prompt.GetComponent<Renderer>().material.color = Color.yellow;
        
        // Set NPC script variables via SerializedObject for proper serialization
        SerializedObject serializedNPC = new SerializedObject(npcScript);
        serializedNPC.FindProperty("npcName").stringValue = "Tutorial Guide";
        serializedNPC.FindProperty("npcDescription").stringValue = "A helpful guide who can teach you about the game.";
        serializedNPC.FindProperty("interactionPrompt").objectReferenceValue = prompt;
        
        // Add some basic responses
        SerializedProperty responsesProp = serializedNPC.FindProperty("responses");
        responsesProp.arraySize = 5;
        
        AddResponsePair(responsesProp, 0, "greeting", "Hello there! I'm here to help you learn the game. Try asking me about 'controls', 'objective', or say 'goodbye'.");
        AddResponsePair(responsesProp, 1, "control", "Use WASD to move around, and press E to interact with NPCs like me!");
        AddResponsePair(responsesProp, 2, "objective", "Your objective is to learn the game mechanics and have fun exploring!");
        AddResponsePair(responsesProp, 3, "goodbye", "Farewell! Come back if you have more questions!");
        AddResponsePair(responsesProp, 4, "help", "You can ask me about: controls, objective, or just say goodbye when you're done.");
        
        serializedNPC.ApplyModifiedProperties();
        
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Setting up Chat UI...", 0.8f);
        
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
            uiDocument.rootVisualElement.styleSheets.Add(uss);
        }
        
        // Add ChatUIController component
        ChatUIController chatController = uiDocumentGO.AddComponent<ChatUIController>();
        chatController.enabled = true;
        
        // Set up references
        SerializedObject serializedController = new SerializedObject(chatController);
        serializedController.FindProperty("uiDocument").objectReferenceValue = uiDocument;
        serializedController.FindProperty("chatContainer").objectReferenceValue = uiDocumentGO;
        serializedController.ApplyModifiedProperties();
        
        EditorUtility.DisplayProgressBar("Creating Game Scene", "Saving scene...", 0.9f);
        
        // Save the scene
        string scenePath = Path.Combine("Assets/Scenes", "GameScene.unity");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"Game scene created at {scenePath}. Press Play to test the NPC interaction.");
    }
    
    private static void AddResponsePair(SerializedProperty responsesProp, int index, string keyword, string response)
    {
        SerializedProperty element = responsesProp.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("keyword").stringValue = keyword;
        element.FindPropertyRelative("response").stringValue = response;
    }
}
