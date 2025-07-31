using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the object that physically sits in the world after being 'used' as an item.
/// </summary>
public abstract class SpawnedPowerup : MonoBehaviour, ICollidable {
    public bool HasInit { get; private set; }
    
    public virtual void Init(KartEntity spawner) { }

    private void Start() {
        HasInit = true;
    }

    public virtual bool Collide(KartEntity kart) {
        return false;
    }
}