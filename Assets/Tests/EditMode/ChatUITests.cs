using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

/// <summary>
/// Basic tests for the Chat UI implementation
/// </summary>
public class ChatUITests
{
    [Test]
    public void ChatUI_CanCreateUIDocument()
    {
        // Create UI Document GameObject
        GameObject gameObject = new GameObject("TestUIDocument");
        UIDocument uiDocument = gameObject.AddComponent<UIDocument>();
        
        // Check it was created correctly
        Assert.IsNotNull(uiDocument, "UIDocument component should be created");
        
        // Clean up
        Object.DestroyImmediate(gameObject);
    }
    
    [Test]
    public void ChatMessage_StoresCorrectValues()
    {
        // Arrange - Create a test message
        string testContent = "Hello, this is a test";
        
        // Act - Create a chat message
        ChatMessage message = new ChatMessage(testContent, ChatMessage.MessageType.User);
        
        // Assert - Check values are stored correctly
        Assert.AreEqual(testContent, message.Content, "Message content should match");
        Assert.AreEqual(ChatMessage.MessageType.User, message.Type, "Message type should match");
        Assert.IsTrue(message.Timestamp <= System.DateTime.Now, "Timestamp should be valid");
    }
}
