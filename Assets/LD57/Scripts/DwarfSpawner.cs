using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DwarfSpawner : MonoBehaviour
{
    public List<CounterPosition> spawnPoints;
    public GameObject dwarfPrefab;

    public float spawnRate;
    public float waitTime;

    float spawnTime;

    private void Awake()
    {
        spawnPoints = FindObjectsByType<CounterPosition>(FindObjectsSortMode.None).ToList();
        spawnTime = spawnRate;
    }

    private void Update()
    {
        spawnTime -= Time.deltaTime;

        if (spawnTime < 0)
        {
            spawnTime = Random.Range(spawnRate, spawnRate * 2);
            if (spawnPoints.Where(f => f.dwarf == null).Count() > 0)
            {
                SpawnDwarf();
            }
        }
    }

    private void SpawnDwarf()
    {
        var emptySpot = spawnPoints.Where(f => f.dwarf == null).OrderBy(o => System.Guid.NewGuid()).FirstOrDefault();
        var dwarf = Instantiate(dwarfPrefab, emptySpot.dwarfSpawnPosition.position, Quaternion.identity);
        emptySpot.AddDwarf(dwarf, waitTime);
    }
}
