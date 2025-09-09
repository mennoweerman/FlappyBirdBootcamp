using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BirdController : MonoBehaviour
{
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public float forwardSpeed;
    public float speed;
    public float jumpForce;

    // UI object to display winning text.
    public GameObject winTextObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        winTextObject.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, 0.0f);
        rb.AddForce(movement * speed);
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            LoseGame();
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        Debug.Log("Move action triggered: " + movementVector);
    }

    void OnJump()
    {
        Debug.Log("Jump action triggered");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    public void LoseGame()
    {
        // Update the winText to display "You Lose!"
        winTextObject.gameObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        
        // Freeze the game
        PauseGame();
        //FindObjectOfType<CountdownTimer>().StopTimer();
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }
}
