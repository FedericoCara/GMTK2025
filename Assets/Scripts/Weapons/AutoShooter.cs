using UnityEngine;

namespace Weapons
{
    public class AutoShooter : MonoBehaviour
    {
        public ShooterType shooterType;
        public Bullet bullet;
        public float shootFrequency = 1f;
        private float nextShootDelay;
        private Transform bulletSpawn;
        private Rigidbody _rb;
        private bool _raceStarted;
    
        void Start()
        {
            _rb = GetComponentInParent<Rigidbody>();
            InitBulletSpawn();
            ResetDelay();
            GameFlowManager.Instance.OnRaceStarted += () => _raceStarted = true;
        }

        private void InitBulletSpawn()
        {
            var bulletSpawnGo = GameObject.Find("BulletHell");
            if (bulletSpawnGo == null)
            {
                bulletSpawnGo = new GameObject("BulletHell");
            }
            bulletSpawn = bulletSpawnGo.transform;
        }


        void Update()
        {
            if(!_raceStarted)
                return;

            nextShootDelay -= Time.deltaTime;

            if (nextShootDelay <= 0)
            {
                ResetDelay();
                SpawnShoot();
            }
        }

        private void SpawnShoot()
        {
            var spawnedBullet = Instantiate(bullet, bulletSpawn);
            spawnedBullet.Init(transform, _rb);
        }

        private void ResetDelay()
        {
            nextShootDelay = shootFrequency;
        }
    }

    public enum ShooterType
    {
        STARTING,
        PIERCING,
        EXPLOSIVE
    }
}