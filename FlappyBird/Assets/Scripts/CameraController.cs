using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    
    [Header("Screen Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.3f;
    
    private float shakeTimer = 0f;
    private Vector3 originalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.transform.position;
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player)
        {
            return;
        }
        
        Vector3 targetPosition = player.transform.position + offset;
        
        // Apply screen shake if active
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;
            
            // Generate random shake offset
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            shakeOffset.z = 0; // Keep shake only on X and Y axis
            
            transform.position = targetPosition + shakeOffset;
        }
        else
        {
            transform.position = targetPosition;
        }
    }
    
    public void StartScreenShake()
    {
        shakeTimer = shakeDuration;
    }
}