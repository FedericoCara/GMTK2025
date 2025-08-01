using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{

    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int initialEnemyPower = 3;
    [SerializeField] private float multiplierEnemyPower = 2f;
    [SerializeField] private float currentEnemyPower;
    [SerializeField] private Objective objective;

    private void Start()
    {
        objective.OnLapCompleted += SpawnEnemies;
        objective.OnStartInitialLap += SpawnEnemies;

        currentEnemyPower = initialEnemyPower;
    }

    private void OnDestroy()
    {
        if (objective)
        {
            objective.OnLapCompleted -= SpawnEnemies;
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < currentEnemyPower; i++)
        {
            Instantiate(
                enemies[Random.Range(0,enemies.Count)], 
                spawnPoints[Random.Range(0,spawnPoints.Count)].position, 
                Quaternion.identity,
                enemiesParent);
        }

        currentEnemyPower *= multiplierEnemyPower;
    }
}
