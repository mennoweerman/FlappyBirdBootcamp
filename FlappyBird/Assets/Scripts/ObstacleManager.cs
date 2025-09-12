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

    void Start()
    {
        activeObstacles = new List<GameObject>();
        
        SpawnInitialObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        MoveObstaclesTowardsPlayer();
    }
    
    void SpawnInitialObstacles()
    {
        if (obstacles.Count == 0) return;
        
        for (int i = 0; i < maxObstacles; i++)
        {
            GameObject obstacleToSpawn = GetUnusedObstacle();
            
            // Bereken spawn positie (startpositie + afstand * index)
            Vector3 spawnPos = spawnPosition + new Vector3(0, 0, spaceBetweenObstacles * i);
            
            GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPos, Quaternion.identity);
            activeObstacles.Add(spawnedObstacle);
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
        
        // Return een willekeurig obstacle uit de beschikbare obstacles
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
        
        // Bereken spawn positie (startpositie + afstand * maxObstacles)
        Vector3 newSpawnPosition = spawnPosition + new Vector3(0, 0, spaceBetweenObstacles * maxObstacles);
        
        GameObject spawnedObstacle = Instantiate(obstacleToSpawnObj, newSpawnPosition, Quaternion.identity);
        activeObstacles.Add(spawnedObstacle);
        
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
        
        // Eerst proberen direct te vinden
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
                // Verwijder null references uit de lijst
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            // Bereken richting naar de speler alleen op de Z-as
            float directionZ = (player.transform.position.z - 10f) - obstacle.transform.position.z;
            Vector3 moveDirection = new Vector3(0, 0, Mathf.Sign(directionZ));
            
            // Beweeg obstacle richting speler alleen op de Z-as met de ingestelde speed
            obstacle.transform.position += moveDirection * obstacleSpeed * Time.deltaTime;
        }
    }
}
