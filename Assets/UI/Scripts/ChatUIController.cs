using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class ChatUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    [Inject] private ILogService logService;
    
    // UI Elements
    private TextField inputField;
    private Button sendButton;
    private ScrollView scrollView;
    private VisualElement chatMessages;
    
    // Chat history for potential persistence
    private List<ChatMessage> messageHistory = new List<ChatMessage>();
    
    private void OnEnable()
    {
        // Get root element
        VisualElement root = uiDocument.rootVisualElement;
        
        // Query UI elements
        inputField = root.Q<TextField>("chat-input-field");
        sendButton = root.Q<Button>("send-button");
        scrollView = root.Q<ScrollView>("chat-scroll-view");
        chatMessages = root.Q<VisualElement>("chat-messages");
        
        // Register event handlers
        sendButton.clicked += OnSendButtonClicked;
        inputField.RegisterCallback<KeyDownEvent>(OnInputFieldKeyDown);
        
        logService.Log("Chat UI initialized");
    }
    
    private void OnDisable()
    {
        // Unregister event handlers to prevent memory leaks
        if (sendButton != null)
            sendButton.clicked -= OnSendButtonClicked;
            
        if (inputField != null)
            inputField.UnregisterCallback<KeyDownEvent>(OnInputFieldKeyDown);
    }
    
    private void OnSendButtonClicked()
    {
        SendMessage();
    }
    
    private void OnInputFieldKeyDown(KeyDownEvent evt)
    {
        // Send message when pressing Enter
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            evt.StopPropagation();
            SendMessage();
        }
    }
    
    private void SendMessage()
    {
        string messageText = inputField.text.Trim();
        
        if (string.IsNullOrEmpty(messageText))
            return;
            
        // Add user message to UI
        AddMessageToUI(messageText, true);
        
        // Store in history
        messageHistory.Add(new ChatMessage(messageText, true));
        
        // Clear input field
        inputField.value = string.Empty;
        inputField.Focus();
        
        // Log for debugging
        logService.Log($"User message: {messageText}");
        
        // Simulate NPC response (will be replaced with actual AI integration)
        SimulateNPCResponse(messageText);
    }
    
    private void SimulateNPCResponse(string userMessage)
    {
        // Simple echo response for testing
        string response = $"You said: {userMessage}";
        
        // Add NPC message to UI with a slight delay to feel more natural
        Invoke(nameof(AddNPCResponse), 0.5f);
        
        // Store response text for the delayed method
        PlayerPrefs.SetString("temp_response", response);
    }
    
    private void AddNPCResponse()
    {
        string response = PlayerPrefs.GetString("temp_response", "I didn't catch that.");
        
        // Add NPC message to UI
        AddMessageToUI(response, false);
        
        // Store in history
        messageHistory.Add(new ChatMessage(response, false));
        
        // Log for debugging
        logService.Log($"NPC response: {response}");
    }
    
    private void AddMessageToUI(string messageText, bool isUserMessage)
    {
        // Create message container
        VisualElement messageContainer = new VisualElement();
        messageContainer.AddToClassList("chat-message");
        
        // Add appropriate styling based on sender
        if (isUserMessage)
            messageContainer.AddToClassList("user-message");
        else
            messageContainer.AddToClassList("npc-message");
        
        // Create and style the message text
        Label messageLabel = new Label(messageText);
        messageContainer.Add(messageLabel);
        
        // Add to the chat window
        chatMessages.Add(messageContainer);
        
        // Scroll to the bottom to show latest message
        scrollView.scrollOffset = new Vector2(0, float.MaxValue);
    }
}

// Simple data class for chat messages
public class ChatMessage
{
    public string Text { get; private set; }
    public bool IsUserMessage { get; private set; }
    public System.DateTime Timestamp { get; private set; }
    
    public ChatMessage(string text, bool isUserMessage)
    {
        Text = text;
        IsUserMessage = isUserMessage;
        Timestamp = System.DateTime.Now;
    }
}
