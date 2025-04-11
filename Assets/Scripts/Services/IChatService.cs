using System;
using System.Threading.Tasks;

public interface IChatService
{
    /// <summary>
    /// Sends a message to the chat service and returns the response
    /// </summary>
    /// <param name="message">The user message to send</param>
    /// <param name="callback">Callback that will be invoked when response is received</param>
    void SendMessage(string message, Action<string> callback);
    
    /// <summary>
    /// Async version that sends a message and returns the response as a Task
    /// </summary>
    /// <param name="message">The user message to send</param>
    /// <returns>Task containing the response string</returns>
    Task<string> SendMessageAsync(string message);
    
    /// <summary>
    /// Clears the current conversation history
    /// </summary>
    void ClearConversation();
}
