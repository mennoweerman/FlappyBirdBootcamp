using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BirdController : MonoBehaviour
{
    private Rigidbody rb;

    private float movementX;

    [Header("Bird Movement Settings")]
    public float speed;

    [Header("Jump Settings")]
    public float jumpForce;
    
    // Rotation settings
    public float baseZRotation = -90f; // Default Z rotation (degrees)
    public float maxRotationRange = 45f; // Maximum extra rotation from base
    public float rotationSpeed = 90f; // How fast the bird rotates

    [Header("Object References")]
    // UI object to display winning text.
    public GameObject winTextObject;
    
    // Reference to ObstacleManager
    public ObstacleManager obstacleManager;
    
    // Reference to HUDController
    public HUDController hudController;
    
    // Reference to JumpParticleController
    public JumpParticleController jumpParticleController;
    
    // Reference to CameraController for screen shake
    public CameraController cameraController;

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public float jumpSoundVolume = .7f;
    public AudioClip scoreSound;
    public float scoreSoundVolume = .7f;
    public AudioClip loseSound;
    public float loseSoundVolume = .7f;

    [Header("Sound Effect Pitch Range")]
    public float soundFXminPitch = 0.6f;
    public float soundFXmaxPitch = 1.25f;
    
    [Header("Slow Motion Settings")]
    public float nearMissDistance = 2f; // Distance to trigger slow motion
    public float slowMotionScale = 0.3f; // How slow the game becomes
    public float slowMotionDuration = 0.5f; // How long slow motion lasts
    
    private bool isSlowMotionActive = false;
    private float slowMotionTimer = 0f;

    private AudioSource audioSource;

    // Score tracking
    private int currentScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        winTextObject.SetActive(false);
        
        // Set initial rotation to base Z rotation
        transform.rotation = Quaternion.Euler(0, -90, baseZRotation);
        
        // Initialize score display
        currentScore = 0;
        if (hudController != null)
        {
            hudController.UpdateScore(currentScore);
        }

        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        // Handle bird rotation based on vertical velocity
        HandleBirdRotation();
        
        // Handle slow motion effect
        HandleSlowMotion();
        
        // Check for near miss with obstacles
        CheckNearMiss();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, 0.0f);
        rb.AddForce(movement * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Border") || collision.gameObject.CompareTag("Obstacle"))
        {
            // Trigger screen shake
            if (cameraController != null)
            {
                cameraController.StartScreenShake();
            }
            
            LoseGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ObstacleClearCollider"))
        {
            if (obstacleManager != null)
            {
                // Vind het parent obstacle van de collider
                GameObject obstacleToRemove = other.gameObject.transform.parent?.gameObject ?? other.gameObject;
                obstacleManager.RemoveObstacleAndSpawnNew(obstacleToRemove);
                
                // Update score through HUD Controller
                if (hudController != null)
                {
                    currentScore++;
                    hudController.UpdateScore(currentScore);
                }
            }
            else
            {
                Debug.LogWarning("ObstacleManager reference is missing in BirdController!");
            }
        }
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
    }

    void OnJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpParticleController.PlayBurst(25);
        if (audioSource != null && jumpSound != null)
        {
            audioSource.pitch = Random.Range(soundFXminPitch, soundFXmaxPitch);
            audioSource.volume = jumpSoundVolume;
            audioSource.PlayOneShot(jumpSound);
        }
    }
    
    void HandleBirdRotation()
    {
        // Calculate target rotation based on vertical velocity
        float targetZRotation = baseZRotation; // Start from base rotation
        
        if (rb.linearVelocity.y > 0) // Bird is moving upward (jumping)
        {
            // Rotate backwards (add to base rotation)
            targetZRotation = baseZRotation + maxRotationRange;
        }
        else if (rb.linearVelocity.y < 0) // Bird is falling
        {
            // Rotate forward (subtract from base rotation) based on fall speed
            float fallSpeedFactor = Mathf.Clamp01(-rb.linearVelocity.y / 10f); // Normalize fall speed
            targetZRotation = baseZRotation - (maxRotationRange * fallSpeedFactor);
        }
        
        // Smoothly rotate towards target rotation on Z-axis
        float currentZRotation = transform.eulerAngles.z;
        
        // Handle angle wrapping (Unity uses 0-360 degrees)
        if (currentZRotation > 180f)
        {
            currentZRotation -= 360f;
        }
        
        float newZRotation = Mathf.MoveTowards(currentZRotation, targetZRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, -90, newZRotation);
    }
    
    void HandleSlowMotion()
    {
        if (isSlowMotionActive)
        {
            slowMotionTimer -= Time.unscaledDeltaTime;
            
            if (slowMotionTimer <= 0f)
            {
                // End slow motion
                isSlowMotionActive = false;
                Time.timeScale = 1f;
            }
        }
    }
    
    void CheckNearMiss()
    {
        if (isSlowMotionActive) return; // Don't trigger if already in slow motion
        
        // Find closest obstacle
        GameObject closestObstacle = FindClosestObstacle();
        
        if (closestObstacle != null)
        {
            float distance = Vector3.Distance(transform.position, closestObstacle.transform.position);
            
            if (distance <= nearMissDistance)
            {
                StartSlowMotion();
                
                // Play score sound for near miss
                if (audioSource != null && scoreSound != null)
                {
                    audioSource.pitch = Random.Range(soundFXminPitch, soundFXmaxPitch);
                    audioSource.volume = scoreSoundVolume;
                    audioSource.PlayOneShot(scoreSound);
                }
            }
        }
    }
    
    GameObject FindClosestObstacle()
    {
        if (obstacleManager == null) return null;
        
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        
        // Get obstacles from obstacle manager (we need to access the active obstacles list)
        // For now, we'll use FindGameObjectsWithTag as a fallback
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Border");
        
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null)
            {
                float distance = Vector3.Distance(transform.position, obstacle.transform.position);
                
                // Only consider obstacles that are ahead of the player
                if (obstacle.transform.position.z > transform.position.z && distance < closestDistance)
                {
                    closest = obstacle;
                    closestDistance = distance;
                }
            }
        }
        
        return closest;
    }
    
    void StartSlowMotion()
    {
        isSlowMotionActive = true;
        slowMotionTimer = slowMotionDuration;
        Time.timeScale = slowMotionScale;
    }
    
    public void LoseGame()
    {
        // Reset time scale in case slow motion was active
        Time.timeScale = 1f;
        isSlowMotionActive = false;
        
        audioSource.pitch = Random.Range(soundFXminPitch, soundFXmaxPitch);
        audioSource.volume = loseSoundVolume;
        audioSource.PlayOneShot(loseSound);

        if (hudController != null)
        {
            hudController.GameOver();
        }
        else
        {
            // Fallback to old method if HUD Controller is not set
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            PauseGame();
        }
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
