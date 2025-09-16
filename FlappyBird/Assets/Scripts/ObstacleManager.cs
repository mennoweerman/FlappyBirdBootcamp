using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public List<GameObject> obstacles;
    private List<GameObject> activeObstacles;
    
    public GameObject player;
    public int maxObstacles = 4;
    public float obstacleSpeed;
    public Vector3 spawnPosition;
    public float spaceBetweenObstacles = 10f;
    
    [Header("Difficulty Settings")]
    public float baseObstacleSpeed = 5f; // Start snelheid
    public float maxObstacleSpeed = 10f; // Maximum snelheid (nog verder verlaagd)
    public float difficultyIncreaseRate = 0.02f; // Snelheidstoename per seconde (zeer langzaam)
    public float spaceDecreaseRate = 0.01f; // Hoe snel ruimte afneemt per seconde (extreem langzaam)
    public float minSpaceMultiplier = 0.8f; // Minimum ruimte als percentage van basis (0.8 = 80% van origineel)
    
    [Header("Powerup Settings")]
    public GameObject powerupPrefab; // Powerup GameObject om te spawnen
    [Range(0, 100)]
    public float powerupSpawnChance = 30f; // Percentage kans om powerup te spawnen (0-100)
    public int powerupPointValue = 5; // Hoeveel punten je krijgt voor een powerup
    public Vector3 powerupRotation = Vector3.zero; // Aangepaste rotatie voor powerups (in graden)
    
    private float gameTime = 0f; // Houd bij hoe lang het spel al bezig is
    private float currentSpaceBetweenObstacles; // Dynamische ruimte die verandert over tijd  

    void Start()
    {
        activeObstacles = new List<GameObject>();
        
        // Initialiseer difficulty instellingen
        obstacleSpeed = baseObstacleSpeed;
        currentSpaceBetweenObstacles = spaceBetweenObstacles; // Begin met basis ruimte
        gameTime = 0f;
        
        SpawnInitialObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDifficulty();
        MoveObstaclesTowardsPlayer();
    }
    
    void UpdateDifficulty()
    {
        // Verhoog alleen difficulty wanneer spel loopt (timeScale > 0)
        if (Time.timeScale > 0)
        {
            gameTime += Time.deltaTime;
            
            // Verhoog obstacle snelheid over tijd
            float targetSpeed = baseObstacleSpeed + (gameTime * difficultyIncreaseRate);
            obstacleSpeed = Mathf.Min(targetSpeed, maxObstacleSpeed);
            
            // Verminder ruimte tussen obstacles over tijd
            float spaceReduction = gameTime * spaceDecreaseRate;
            float minSpace = spaceBetweenObstacles * minSpaceMultiplier;
            currentSpaceBetweenObstacles = Mathf.Max(spaceBetweenObstacles - spaceReduction, minSpace);
        }
    }
    
    void SpawnInitialObstacles()
    {
        if (obstacles.Count == 0) return;
        
        for (int i = 0; i < maxObstacles; i++)
        {
            GameObject obstacleToSpawn = GetUnusedObstacle();
            
            // Bereken spawn positie (startpositie + afstand * index)
            Vector3 spawnPos = spawnPosition + new Vector3(0, 0, currentSpaceBetweenObstacles * i);
            
            GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPos, Quaternion.identity, transform);
            activeObstacles.Add(spawnedObstacle);
            
            // Probeer powerup te spawnen op dit obstacle
            TrySpawnPowerup(spawnedObstacle);
        }
    }
    
    GameObject GetUnusedObstacle()
    {
        // Maak een lijst van nog niet actieve obstacle types
        List<GameObject> availableObstacles = new List<GameObject>();
        
        foreach (GameObject obstacle in obstacles)
        {
            // Check of dit obstacle type al actief is
            bool isAlreadyActive = false;
            foreach (GameObject activeObstacle in activeObstacles)
            {
                string activeName = activeObstacle.name.Replace("(Clone)", "");
                if (activeName == obstacle.name)
                {
                    isAlreadyActive = true;
                    break;
                }
            }
            
            if (!isAlreadyActive)
            {
                availableObstacles.Add(obstacle);
            }
        }
        
        // Als er geen ongebruikte obstacles zijn, return null
        if (availableObstacles.Count == 0)
        {
            return null;
        }
        
        // Geef een willekeurig obstacle terug uit de beschikbare obstacles
        int randomIndex = Random.Range(0, availableObstacles.Count);
        return availableObstacles[randomIndex];
    }
    
    public void SpawnObstacle()
    {
        if (obstacles.Count == 0) return; 
        
        GameObject obstacleToSpawnObj = GetUnusedObstacle();
        
        if (obstacleToSpawnObj == null)
        {
            // Als alle obstacle types al actief zijn, kies willekeurig
            int randomIndex = Random.Range(0, obstacles.Count);
            obstacleToSpawnObj = obstacles[randomIndex];
        }
        
        // Vind de laatste (verste) obstacle positie
        Vector3 newSpawnPosition = spawnPosition;
        if (activeObstacles.Count > 0)
        {
            float farthestZ = float.MinValue;
            foreach (GameObject activeObstacle in activeObstacles)
            {
                if (activeObstacle != null && activeObstacle.transform.position.z > farthestZ)
                {
                    farthestZ = activeObstacle.transform.position.z;
                }
            }
            
            // Spawn achter het laatste obstacle + currentSpaceBetweenObstacles
            newSpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, farthestZ + currentSpaceBetweenObstacles);
        }
        
        GameObject spawnedObstacle = Instantiate(obstacleToSpawnObj, newSpawnPosition, Quaternion.identity, transform);
        activeObstacles.Add(spawnedObstacle);
        
        // Probeer powerup te spawnen op dit obstacle
        TrySpawnPowerup(spawnedObstacle);
        
        // Verwijder het oudste obstacle als we meer dan maxObstacles hebben
        if (activeObstacles.Count > maxObstacles)
        {
            GameObject oldestObstacle = activeObstacles[0];
            activeObstacles.RemoveAt(0);
            Destroy(oldestObstacle);
        }
    }
    
    public void RemoveObstacleAndSpawnNew(GameObject obstacleToRemove)
    {
        
        // Zoek het obstacle in de activeObstacles lijst
        GameObject obstacleInList = null;
        
        // Probeer eerst direct te vinden
        if (activeObstacles.Contains(obstacleToRemove))
        {
            obstacleInList = obstacleToRemove;
        }
        else
        {
            // Als het niet direct gevonden wordt, zoek via parent/child relatie
            foreach (GameObject activeObstacle in activeObstacles)
            {
                if (activeObstacle == obstacleToRemove || 
                    (obstacleToRemove.transform.parent != null && activeObstacle == obstacleToRemove.transform.parent.gameObject) ||
                    activeObstacle.transform.IsChildOf(obstacleToRemove.transform) ||
                    obstacleToRemove.transform.IsChildOf(activeObstacle.transform))
                {
                    obstacleInList = activeObstacle;
                    break;
                }
            }
        }
        
        if (obstacleInList != null)
        {
            activeObstacles.Remove(obstacleInList);
            Destroy(obstacleInList);
            
            SpawnObstacle();
        }
        else
        {
            SpawnObstacle();
        }
    }
    
    void MoveObstaclesTowardsPlayer()
    {
        if (player == null) return;
        
        // Loop door alle actieve obstacles
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = activeObstacles[i];
            
            if (obstacle == null)
            {
                // Verwijder null referenties uit de lijst
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            // Bereken richting naar de speler alleen op de Z-as
            float directionZ = (player.transform.position.z - 10f) - obstacle.transform.position.z;
            Vector3 moveDirection = new Vector3(0, 0, Mathf.Sign(directionZ));
            
            // Beweeg obstacle richting speler alleen op de Z-as met de ingestelde snelheid
            obstacle.transform.position += moveDirection * obstacleSpeed * Time.deltaTime;
        }
    }
    
    public void ResetDifficulty()
    {
        // Reset moeilijkheidsgraad naar startwaarden
        gameTime = 0f;
        obstacleSpeed = baseObstacleSpeed;
        currentSpaceBetweenObstacles = spaceBetweenObstacles;
    }
    
    void TrySpawnPowerup(GameObject obstacle)
    {
        // Check of powerup prefab is ingesteld
        if (powerupPrefab == null) return;
        
        // Genereer random getal tussen 0-100
        float randomValue = Random.Range(0f, 100f);
        
        // Spawn powerup als random waarde hoger is dan spawn kans
        if (randomValue > (100f - powerupSpawnChance))
        {
            // Zoek naar ItemSpawnPoint in het obstacle
            Transform itemSpawnPoint = FindItemSpawnPoint(obstacle.transform);
            
            if (itemSpawnPoint != null)
            {
                // Spawn powerup op de ItemSpawnPoint positie met aangepaste rotatie
                Quaternion spawnRotation = Quaternion.Euler(powerupRotation);
                GameObject spawnedPowerup = Instantiate(powerupPrefab, itemSpawnPoint.position, spawnRotation, obstacle.transform);
                
                // Zorg ervoor dat de powerup de "PickUpItem" tag heeft
                if (spawnedPowerup.CompareTag("Untagged"))
                {
                    spawnedPowerup.tag = "PickUpItem";
                }
            }
            else
            {
                Debug.LogWarning("ItemSpawnPoint niet gevonden in obstacle: " + obstacle.name);
            }
        }
    }
    
    Transform FindItemSpawnPoint(Transform parent)
    {
        // Zoek recursief naar een child object genaamd "ItemSpawnPoint"
        if (parent.name == "ItemSpawnPoint")
        {
            return parent;
        }
        
        foreach (Transform child in parent)
        {
            Transform found = FindItemSpawnPoint(child);
            if (found != null)
            {
                return found;
            }
        }
        
        return null;
    }
}
