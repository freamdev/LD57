using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    public GameObject orePrefab;
    public Transform spawnPosition;
    public float spawnRatio;
    public int spawnCount;

    float spawnTime;

    private void Awake()
    {
        ResetSpawnTime();
    }

    void ResetSpawnTime()
    {
        spawnTime = Random.Range(spawnRatio, spawnRatio * 2);
    }

    private void Update()
    {
        spawnTime -= Time.deltaTime;

        if (spawnTime < 0)
        {
            ResetSpawnTime();
            for (int i = 0; i < spawnCount; i++)
            {
                Instantiate(orePrefab, spawnPosition.position, Quaternion.identity);
            }
        }
    }
}
