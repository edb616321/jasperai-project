using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the chat UI interaction using UI Toolkit
/// </summary>
public class ChatUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    // UI Elements
    private TextField inputField;
    private Button sendButton;
    private ScrollView scrollView;
    private VisualElement chatMessages;
    
    // Store chat history
    private List<ChatMessage> messageHistory = new List<ChatMessage>();
    
    private void OnEnable()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument reference missing on ChatUIController");
            return;
        }
        
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
        
        // Add welcome message
        AddSystemMessage("Welcome to the chat! How can I help you today?");
        
        Debug.Log("Chat UI initialized");
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
        SendUserMessage();
    }
    
    private void OnInputFieldKeyDown(KeyDownEvent evt)
    {
        // Send message when pressing Enter (without Shift for newline)
        if ((evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && !evt.shiftKey)
        {
            evt.StopPropagation();
            SendUserMessage();
        }
    }
    
    private void SendUserMessage()
    {
        string messageText = inputField.text.Trim();
        
        if (string.IsNullOrEmpty(messageText))
            return;
            
        // Add user message to UI
        AddMessageToUI(messageText, ChatMessage.MessageType.User);
        
        // Store in history
        messageHistory.Add(new ChatMessage(messageText, ChatMessage.MessageType.User));
        
        // Clear input field
        inputField.value = string.Empty;
        inputField.Focus();
        
        // Log message for debugging
        Debug.Log($"User message: {messageText}");
        
        // Process the message (simple echo response for now)
        ProcessMessage(messageText);
    }
    
    private void ProcessMessage(string userMessage)
    {
        // Simple echo response for testing - this would be replaced with actual AI logic
        string response = $"You said: {userMessage}";
        
        // Add a slight delay to feel more natural
        Invoke(nameof(AddResponseMessage), 0.8f);
        
        // Store response for delayed method
        PlayerPrefs.SetString("temp_response", response);
    }
    
    private void AddResponseMessage()
    {
        string response = PlayerPrefs.GetString("temp_response", "I didn't catch that.");
        
        // Add AI message to UI
        AddMessageToUI(response, ChatMessage.MessageType.AI);
        
        // Store in history
        messageHistory.Add(new ChatMessage(response, ChatMessage.MessageType.AI));
        
        // Log for debugging
        Debug.Log($"AI response: {response}");
    }
    
    private void AddSystemMessage(string message)
    {
        AddMessageToUI(message, ChatMessage.MessageType.System);
        messageHistory.Add(new ChatMessage(message, ChatMessage.MessageType.System));
    }
    
    private void AddMessageToUI(string messageText, ChatMessage.MessageType type)
    {
        // Create message container
        VisualElement messageContainer = new VisualElement();
        messageContainer.AddToClassList("chat-message");
        
        // Add appropriate styling based on message type
        switch (type)
        {
            case ChatMessage.MessageType.User:
                messageContainer.AddToClassList("user-message");
                break;
            case ChatMessage.MessageType.AI:
                messageContainer.AddToClassList("npc-message");
                break;
            case ChatMessage.MessageType.System:
                messageContainer.AddToClassList("system-message");
                break;
        }
        
        // Create and style the message text
        Label messageLabel = new Label(messageText);
        messageContainer.Add(messageLabel);
        
        // Add timestamp if not a system message
        if (type != ChatMessage.MessageType.System)
        {
            Label timestampLabel = new Label(System.DateTime.Now.ToString("HH:mm"));
            timestampLabel.AddToClassList("timestamp");
            messageContainer.Add(timestampLabel);
        }
        
        // Add to the chat window
        chatMessages.Add(messageContainer);
        
        // Scroll to the bottom to show latest message
        scrollView.scrollOffset = new Vector2(0, float.MaxValue);
    }
}

/// <summary>
/// Simple data class for chat messages
/// </summary>
public class ChatMessage
{
    public enum MessageType
    {
        User,
        AI,
        System
    }
    
    public string Content { get; private set; }
    public MessageType Type { get; private set; }
    public System.DateTime Timestamp { get; private set; }
    
    public ChatMessage(string content, MessageType type)
    {
        Content = content;
        Type = type;
        Timestamp = System.DateTime.Now;
    }
}
