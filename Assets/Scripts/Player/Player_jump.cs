using System;
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
    private bool isGrounded = true; // Starts as true to allow the first jump

    [SerializeField] private Button jumpButton; // UI Button (Assigned in Inspector)

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
        // Keyboard Jump Support (For PC)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void OnJumpButtonPressed()
    {
        Jump(); // Calls the Jump function when UI button is pressed
    }

    void Jump()
    {
        Console.WriteLine("pressed");
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
}