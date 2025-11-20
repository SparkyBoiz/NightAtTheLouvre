using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The speed at which the player moves.")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found! Please attach one for movement.");
        }

        if (animator == null)
        {
            Debug.LogWarning("Animator component not found in parent. Animations will not play.");
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
        if (animator != null)
        {
            if (moveDirection.magnitude > 0)
            {
                animator.SetBool("IsMoving", true); // This will now work correctly
            }
            else
            {
                animator.SetBool("IsMoving", false); // This will now work correctly
            }
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector2 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }
}
