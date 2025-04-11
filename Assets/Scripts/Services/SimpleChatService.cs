using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;

/// <summary>
/// Simple implementation of IChatService that generates responses locally
/// This will be replaced with actual AI integration in the future
/// </summary>
public class SimpleChatService : IChatService
{
    [Inject] private ILogService logService;
    
    // Store conversation history
    private List<string> conversationHistory = new List<string>();
    
    // Sample responses for quick testing
    private readonly string[] sampleResponses = new string[]
    {
        "I'm just a simple NPC. My AI capabilities will be enhanced in future updates.",
        "That's interesting! Tell me more about that.",
        "I'm not sure I understand. Could you rephrase that?",
        "I'm here to help you with basic information about this world.",
        "What else would you like to know?"
    };
    
    public void SendMessage(string message, Action<string> callback)
    {
        if (string.IsNullOrEmpty(message))
        {
            callback?.Invoke("I didn't catch that. Could you please say something?");
            return;
        }
        
        // Log the message
        logService.Log($"Chat service received: {message}");
        
        // Add to conversation history
        conversationHistory.Add($"User: {message}");
        
        // Generate simple response
        string response = GenerateResponse(message);
        
        // Add response to history
        conversationHistory.Add($"NPC: {response}");
        
        // Invoke callback with response
        callback?.Invoke(response);
    }
    
    public async Task<string> SendMessageAsync(string message)
    {
        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
        
        // Use the synchronous version and return the result via TaskCompletionSource
        SendMessage(message, response => tcs.SetResult(response));
        
        // Simulate network delay for more realistic async behavior
        await Task.Delay(300);
        
        return await tcs.Task;
    }
    
    public void ClearConversation()
    {
        conversationHistory.Clear();
        logService.Log("Chat conversation history cleared");
    }
    
    private string GenerateResponse(string userMessage)
    {
        // Basic response generation - could be enhanced with simple pattern matching
        // This is a placeholder for actual AI integration
        
        // Convert to lowercase for simpler matching
        string lowerMessage = userMessage.ToLower();
        
        // Simple pattern matching
        if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("hey"))
        {
            return "Hello there! How can I help you today?";
        }
        else if (lowerMessage.Contains("bye") || lowerMessage.Contains("goodbye"))
        {
            return "Goodbye! Come back soon!";
        }
        else if (lowerMessage.Contains("thank"))
        {
            return "You're welcome! Is there anything else you'd like to know?";
        }
        else if (lowerMessage.Contains("who are you") || lowerMessage.Contains("your name"))
        {
            return "I'm an NPC created to demonstrate UI Toolkit chat integration. My capabilities will be expanded in future updates.";
        }
        else if (lowerMessage.Contains("what can you do") || lowerMessage.Contains("help me"))
        {
            return "Currently I can have simple conversations. In the future, I'll be connected to an AI system to provide more intelligent responses.";
        }
        
        // Default to random response if no pattern matches
        return sampleResponses[UnityEngine.Random.Range(0, sampleResponses.Length)];
    }
}
