using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Basic NPC that can be interacted with and displays dialogue
/// </summary>
public class NPC : MonoBehaviour
{
    [Header("NPC Information")]
    [SerializeField] private string npcName = "Friendly NPC";
    [SerializeField] private string npcDescription = "A helpful character who can provide information.";
    
    [Header("Interaction Settings")]
    [SerializeField] private GameObject interactionPrompt;
    
    [Header("Dialogue")]
    [SerializeField] private string greeting = "Hello there! How can I help you today?";
    [SerializeField] private string farewell = "Goodbye! Come back if you need anything else.";
    
    // Simple responses without complex dialogue trees
    [SerializeField] private List<ResponsePair> responses = new List<ResponsePair>();
    
    private bool isInConversation = false;
    
    public string NPCName => npcName;
    public string NPCDescription => npcDescription;
    
    private void Start()
    {
        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    /// <summary>
    /// Show or hide the interaction prompt above the NPC
    /// </summary>
    public void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
        }
    }
    
    /// <summary>
    /// Start conversation with this NPC
    /// </summary>
    public void StartConversation()
    {
        isInConversation = true;
        ShowInteractionPrompt(false);
        Debug.Log($"{npcName}: {greeting}");
    }
    
    /// <summary>
    /// End conversation with this NPC
    /// </summary>
    public void EndConversation()
    {
        isInConversation = false;
        Debug.Log($"{npcName}: {farewell}");
    }
    
    /// <summary>
    /// Get a response to player's message
    /// </summary>
    public string GetResponse(string playerMessage)
    {
        // Default response if nothing matches
        string defaultResponse = "I'm not sure what you mean by that.";
        
        // Simple keyword matching for responses
        foreach (ResponsePair pair in responses)
        {
            if (playerMessage.ToLower().Contains(pair.keyword.ToLower()))
            {
                return pair.response;
            }
        }
        
        return defaultResponse;
    }
}

/// <summary>
/// Simple keyword-response pair for NPC dialogue
/// </summary>
[System.Serializable]
public class ResponsePair
{
    public string keyword;
    public string response;
}
