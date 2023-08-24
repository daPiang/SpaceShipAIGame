using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    public float offScreenDestroyTime = 5f; // Time in seconds after which the asteroid is destroyed if off-screen
    private bool isOffScreen;               // Flag to determine if asteroid is off screen
    private float offScreenTimer = 0;       // Timer to keep track of how long the asteroid has been off screen

     public delegate void Lose();
     public static event Lose LoseEvent;

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * 10);

        // Check if asteroid is off the screen
        if (IsOffScreen())
        {
            // Start or continue counting
            offScreenTimer += Time.deltaTime;
            if (offScreenTimer >= offScreenDestroyTime)
            {
                Destroy(gameObject);  // Destroy the asteroid after a certain duration off-screen
            }
        }
        else
        {
            offScreenTimer = 0;  // Reset timer if asteroid is back on screen
        }
    }

    private bool IsOffScreen()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        // Check if the position is outside the screen bounds
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("GAMEOVER");
            LoseEvent?.Invoke();
        }
    }
}
