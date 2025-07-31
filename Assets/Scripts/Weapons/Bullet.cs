using System;
using Enemies;
using UnityEngine;

namespace Weapons
{
    public class Bullet: MonoBehaviour
    {
        
        public float speed;
        public int damage = 1;
        private Rigidbody _rb;
        private float _lifeTime;
        private int _trackLayer;
        private Vector3 _spawnerVelocity;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            UpdateVelocity();
            _lifeTime = 10f;
            _trackLayer = LayerMask.GetMask("Track");
        }

        public void Init(Transform spawnerTransform, Rigidbody rb)
        {
            transform.position = spawnerTransform.position;
            transform.rotation = spawnerTransform.rotation;
            _spawnerVelocity = rb.linearVelocity;
        }

        private void Update()
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime < 0)
            {
                Destroy(gameObject);
            }

            AlignWithGround();
        }

        void FixedUpdate()
        {
            AlignWithGround();
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            _rb.linearVelocity = (transform.forward * speed) + _spawnerVelocity;
        }

        private void AlignWithGround(float rayDistance = 2f, float smoothRotation = 10f)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance, _trackLayer))
            {
                Vector3 groundNormal = hit.normal;

                Quaternion targetRotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized,
                    groundNormal
                );

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * smoothRotation
                );
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals(Enemy.EnemyTag))
            {
                other.GetComponentInParent<Enemy>().ReceiveDamage(damage);
                Destroy(gameObject);
            } else if (IsWall(other))
            {
                Destroy(gameObject);
            }
        }

        private static bool IsWall(Collider other) => other.tag.Equals("wall") || LayerMask.LayerToName(other.gameObject.layer).Equals("Track");

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * speed);
        }
    }
}