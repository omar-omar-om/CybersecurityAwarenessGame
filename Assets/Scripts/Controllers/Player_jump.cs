using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce = 10f;
    private int jumpsLeft = 2;  // I start with 2 jumps
    public int maxJumps = 2;
    private bool isOnGround = true;  // Player starts on ground

    [SerializeField] private Button jumpButton; // UI Button (Assigned in Inspector)

    public static bool canJump = true;  // Used to disable jumping during questions

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure Jump Button is assigned and add a listener
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(OnJumpButtonPressed);
        }
    }

    void Update()
    {
        // Prevent jumping if disabled by Question System
        if (!canJump) return;

        // Keyboard Jump Support (For PC)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void OnJumpButtonPressed()
    {
        if (canJump) // Only allow jumping if enabled
        {
            Jump();
        }
    }

    void Jump()
    {
        Console.WriteLine("pressed");
        if (isOnGround || jumpsLeft > 0)  // Can jump if on ground or has jumps left
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft = jumpsLeft - 1;  // Use up one jump
            isOnGround = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpsLeft = maxJumps;  // Get all jumps back when landing
            isOnGround = true;
        }
    }
}
