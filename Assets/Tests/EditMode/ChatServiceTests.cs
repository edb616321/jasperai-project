using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChatServiceTests
{
    private SimpleChatService chatService;
    private MockLogService logService;
    
    [SetUp]
    public void Setup()
    {
        // Create mock services
        logService = new MockLogService();
        
        // Initialize chat service with mock dependencies
        chatService = new SimpleChatService();
        
        // Use reflection to inject dependencies manually (alternative to Zenject in tests)
        var logServiceField = typeof(SimpleChatService).GetField("logService", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        logServiceField?.SetValue(chatService, logService);
    }
    
    [Test]
    public void ChatService_SendsMessage_WithValidResponse()
    {
        // Arrange
        string userMessage = "Hello";
        string response = null;
        
        // Act
        chatService.SendMessage(userMessage, (result) => { response = result; });
        
        // Assert
        Assert.IsNotNull(response, "Response should not be null");
        Assert.IsTrue(response.Length > 0, "Response should not be empty");
        Assert.IsTrue(logService.LoggedMessages.Count > 0, "Message should be logged");
    }
    
    [Test]
    public void ChatService_HandlesEmptyMessage_GracefullyWithResponse()
    {
        // Arrange
        string userMessage = "";
        string response = null;
        
        // Act
        chatService.SendMessage(userMessage, (result) => { response = result; });
        
        // Assert
        Assert.IsNotNull(response, "Empty message should still produce a response");
        Assert.IsTrue(response.Contains("didn't catch"), "Response should indicate empty message");
    }
    
    [Test]
    public void ChatService_ClearsConversation_Successfully()
    {
        // Arrange - Add some messages first
        chatService.SendMessage("Hello", _ => { });
        chatService.SendMessage("How are you?", _ => { });
        
        // Reset log count to verify clearing logs
        logService.LoggedMessages.Clear();
        
        // Act
        chatService.ClearConversation();
        
        // Assert
        Assert.IsTrue(logService.LoggedMessages.Count > 0, "ClearConversation should log an event");
        Assert.IsTrue(logService.LoggedMessages.Exists(msg => msg.Contains("cleared")), 
            "Log should indicate conversation was cleared");
    }
}

/// <summary>
/// Mock implementation of ILogService for testing
/// </summary>
public class MockLogService : ILogService
{
    public List<string> LoggedMessages { get; } = new List<string>();
    
    public void Log(string message)
    {
        LoggedMessages.Add(message);
    }
    
    public void LogWarning(string message)
    {
        LoggedMessages.Add($"WARNING: {message}");
    }
    
    public void LogError(string message)
    {
        LoggedMessages.Add($"ERROR: {message}");
    }
}
