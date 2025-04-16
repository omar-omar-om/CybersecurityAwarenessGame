using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce = 10f;
    private int jumpCount = 0;
    public int maxJumps = 2;
    private bool isGrounded = true; 
    private Animator animator;
    private ParticleSystem playerParticles;
    private float startTime;

    [SerializeField] private Button jumpButton;

    public static bool canJump = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerParticles = GetComponentInChildren<ParticleSystem>();
        startTime = Time.time;

        // Ensure Jump Button is assigned and add a listener
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(OnJumpButtonPressed);
        }

        // Freeze animation and disable particles at start
        animator.speed = 0f;
        if (playerParticles != null)
        {
            playerParticles.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Prevent jumping if less than 3 seconds have passed
        if (Time.time - startTime < 4f) return;

        // Prevent jumping if disabled by Question System
        if (canJump == false) return;

        // Keyboard Jump Support (For PC)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void OnJumpButtonPressed()
    {
        // Prevent jumping if less than 3 seconds have passed
        if (Time.time - startTime < 4f) return;

        if (canJump == true) // Only allow jumping if enabled
        {
            Jump();
        }
    }

    void Jump()
    {
        if (isGrounded || jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false; // Player is airborne after jumping
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            isGrounded = true; // Reset to allow jumping again
        }
    }

    // Call this method when countdown is over
    public void StartAnimation()
    {
        animator.speed = 1f;
        if (playerParticles != null)
        {
            playerParticles.gameObject.SetActive(true);
        }
    }
}
