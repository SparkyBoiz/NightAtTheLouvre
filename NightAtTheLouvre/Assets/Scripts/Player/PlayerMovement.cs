using UnityEngine;
using UnityEngine.InputSystem; // Import the New Input System namespace

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The speed at which the player moves.")]
    public float moveSpeed = 5f;

    // A reference to the Rigidbody2D component
    private Rigidbody2D rb;
    
    // A private variable to store the Vector2 value from the "Move" action
    private Vector2 moveDirection;

    void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found! Please attach one for movement.");
        }

        // Ensure the Rigidbody2D's gravity scale is zero for top-down movement
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // This method is automatically called by the PlayerInput component 
    // when the "Movement" action's value changes (if Behavior is set to "Send Messages").
    // The name 'OnMove' must match the action name in your Input Actions Asset.
    public void OnMove(InputValue value)
    {
        // Read the Vector2 value from the input action (WASD keys map to a 2D Vector)
        moveDirection = value.Get<Vector2>();
    }

    // FixedUpdate is used for physics calculations
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply velocity: direction vector * speed
            // The Vector2 received from the action is already normalized 
            // if you set up the WASD composite correctly in the Input Actions Asset.
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }
}