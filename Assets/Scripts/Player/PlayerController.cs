using UnityEngine;

/// <summary>
/// Simple player controller with standard Unity components
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 120f;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Transform cameraTransform;
    
    private CharacterController characterController;
    private NPC currentNPC;
    private bool isInteracting = false;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // If camera transform is not set, try to find main camera
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("No main camera found. Please assign a camera to the PlayerController.");
            }
        }
    }
    
    private void Update()
    {
        if (!isInteracting)
        {
            HandleMovement();
            HandleInteraction();
        }
        
        // Input to exit conversation
        if (isInteracting && Input.GetKeyDown(KeyCode.Escape))
        {
            EndInteraction();
        }
    }
    
    private void HandleMovement()
    {
        // Get input values
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Apply movement using CharacterController
        if (moveDirection.magnitude > 0.1f)
        {
            // Calculate the angle to rotate
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            
            // Smoothly rotate towards movement direction
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            
            // Move in forward direction
            characterController.SimpleMove(transform.forward * moveSpeed);
        }
    }
    
    private void HandleInteraction()
    {
        // Check for NPC in range
        NPC nearestNPC = FindNearestNPC();
        
        // Show/hide interaction prompt
        if (nearestNPC != null && nearestNPC != currentNPC)
        {
            currentNPC = nearestNPC;
            currentNPC.ShowInteractionPrompt(true);
            Debug.Log($"Can interact with: {currentNPC.NPCName}");
        }
        else if (nearestNPC == null && currentNPC != null)
        {
            currentNPC.ShowInteractionPrompt(false);
            currentNPC = null;
        }
        
        // Handle interaction key press
        if (Input.GetKeyDown(interactKey) && currentNPC != null)
        {
            StartInteraction(currentNPC);
        }
    }
    
    private NPC FindNearestNPC()
    {
        // Simple sphere cast to find NPCs
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        NPC nearestNPC = null;
        float closestDistance = interactionRange;
        
        foreach (Collider collider in colliders)
        {
            NPC npc = collider.GetComponent<NPC>();
            if (npc != null)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestNPC = npc;
                }
            }
        }
        
        return nearestNPC;
    }
    
    private void StartInteraction(NPC npc)
    {
        isInteracting = true;
        currentNPC.StartConversation();
        
        // Activate the chat UI
        ChatUIController chatUI = FindObjectOfType<ChatUIController>();
        if (chatUI != null)
        {
            chatUI.StartConversation(currentNPC);
        }
        else
        {
            Debug.LogError("ChatUIController not found in the scene!");
        }
    }
    
    private void EndInteraction()
    {
        if (currentNPC != null)
        {
            currentNPC.EndConversation();
        }
        
        isInteracting = false;
        
        // Deactivate the chat UI
        ChatUIController chatUI = FindObjectOfType<ChatUIController>();
        if (chatUI != null)
        {
            chatUI.EndConversation();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
