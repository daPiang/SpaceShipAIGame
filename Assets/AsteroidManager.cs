using System.Collections;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public GameObject[] asteroidPrefabs; 
    public int maxOnScreenAsteroids = 1000;
    public float spawnRange = 50f;
    public Vector2 sizeRange = new Vector2(0.5f, 2f);
    public float spawnInterval = 0.5f;
    public Rocket rocket;  // Reference to the Rocket script/component

    public float minAsteroidDistance = 10f;  // Minimum distance between spawned asteroids.

    [Range(0f, 1f)]
    public float directionBias = 0.75f;  // How much to bias the spawn direction towards the rocket's direction. 1 is fully biased, 0 is no bias.

    private int currentAsteroidsCount = 0;

    void Start()
    {
        StartCoroutine(SpawnAsteroidsRoutine());
    }

    IEnumerator SpawnAsteroidsRoutine()
    {
        while (true)
        {
            if (currentAsteroidsCount < maxOnScreenAsteroids)
            {
                SpawnAsteroid();
                currentAsteroidsCount++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAsteroid()
    {
        Vector3 randomPosition;
        int maxAttempts = 10;
        int currentAttempt = 0;

        do
        {
            randomPosition = GetRandomOffScreenPosition(spawnRange);
            randomPosition.z = 0;  // or any other appropriate value

            currentAttempt++;
            if (currentAttempt > maxAttempts)
            {
                Debug.LogWarning("Max spawn attempts reached! Might overlap with camera view or other asteroids.");
                break;
            }
        } while (IsInsideCameraBounds(randomPosition) || IsTooCloseToOtherAsteroids(randomPosition));

        GameObject chosenAsteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        GameObject asteroidInstance = Instantiate(chosenAsteroidPrefab, randomPosition, Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
        float randomSize = Random.Range(sizeRange.x, sizeRange.y);
        asteroidInstance.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
    }

    bool IsTooCloseToOtherAsteroids(Vector3 position)
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(position, minAsteroidDistance);
        foreach (Collider2D col in nearbyObjects)
        {
            if (col.CompareTag("Asteroid"))
            {
                return true;
            }
        }
        return false;
    }

    Vector3 GetRandomOffScreenPosition(float range)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 biasedDirection = rocket.transform.up;
        
        randomDirection = Vector3.Lerp(randomDirection, biasedDirection, directionBias);
        randomDirection.z = 0; // 2D game, so z should be 0
        randomDirection.Normalize();

        return Camera.main.transform.position + randomDirection * (Camera.main.orthographicSize + range);
    }

    bool IsInsideCameraBounds(Vector3 position)
    {
        Vector2 screenPosition = Camera.main.WorldToViewportPoint(position);
        return screenPosition.x >= 0 && screenPosition.x <= 1 && screenPosition.y >= 0 && screenPosition.y <= 1;
    }
}
