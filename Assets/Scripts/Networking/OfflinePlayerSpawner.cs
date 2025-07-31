using UnityEngine;

public class OfflinePlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] kartPrefabs;
    
    private void Start()
    {
        SpawnOfflinePlayer();
    }
    
    private void SpawnOfflinePlayer()
    {
        if (spawnPoints.Length == 0 || kartPrefabs.Length == 0)
        {
            Debug.LogError("No spawn points or kart prefabs configured!");
            return;
        }
        
        // Spawn player at first spawn point with first kart
        Transform spawnPoint = spawnPoints[0];
        GameObject kartPrefab = kartPrefabs[0];
        
        GameObject kart = Instantiate(kartPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Set up the kart for offline mode
        var kartEntity = kart.GetComponent<KartEntity>();
        if (kartEntity != null)
        {
            // Replace KartController with KartControllerOffline if needed
            var originalController = kart.GetComponent<KartController>();
            if (originalController != null)
            {
                var offlineController = kart.AddComponent<KartControllerOffline>();
                // Copy settings from original controller
                CopyControllerSettings(originalController, offlineController);
                Destroy(originalController);
            }
        }
        
        Debug.Log("Offline player spawned successfully!");
    }
    
    private void CopyControllerSettings(KartController original, KartControllerOffline offline)
    {
        // Copy all the serialized fields
        offline.collider = original.collider;
        offline.driftTiers = original.driftTiers;
        offline.model = original.model;
        offline.tireFL = original.tireFL;
        offline.tireFR = original.tireFR;
        offline.tireYawFL = original.tireYawFL;
        offline.tireYawFR = original.tireYawFR;
        offline.tireBL = original.tireBL;
        offline.tireBR = original.tireBR;
        offline.maxSpeedNormal = original.maxSpeedNormal;
        offline.maxSpeedBoosting = original.maxSpeedBoosting;
        offline.reverseSpeed = original.reverseSpeed;
        offline.acceleration = original.acceleration;
        offline.deceleration = original.deceleration;
        offline.steeringCurve = original.steeringCurve;
        offline.maxSteerStrength = original.maxSteerStrength;
        offline.steerAcceleration = original.steerAcceleration;
        offline.steerDeceleration = original.steerDeceleration;
        offline.driftInputRemap = original.driftInputRemap;
        offline.hopSteerStrength = original.hopSteerStrength;
        offline.speedToDrift = original.speedToDrift;
        offline.driftRotationLerpFactor = original.driftRotationLerpFactor;
    }
} 