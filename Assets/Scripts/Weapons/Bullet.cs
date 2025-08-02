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
        }

        void FixedUpdate()
        {
            AlignWithGround(2, 20, 1);
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            _rb.linearVelocity = (transform.forward * speed) + _spawnerVelocity;
        }

        private void AlignWithGround(
            float rayDistance = 2f,
            float smoothRotation = 10f,
            float fixedHeight = 0.5f
        )
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, -transform.up * rayDistance, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out RaycastHit hit, rayDistance, _trackLayer))
            {
                Vector3 groundNormal = hit.normal;

                // --- Rotación alineada con la pendiente ---
                Quaternion targetRotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized,
                    groundNormal
                );

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * smoothRotation
                );

                // Posicionar a altura fija sobre el piso
                Vector3 targetPosition = new Vector3(
                    transform.position.x,
                    hit.point.y + fixedHeight,
                    transform.position.z
                );

                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.fixedDeltaTime * smoothRotation
                );
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals(Enemy.EnemyTag))
            {
                OnImpactedEnemy(other.GetComponentInParent<Enemy>());
            } else if (IsWall(other))
            {
                OnImpactedWall();
            }
        }

        protected virtual void OnImpactedEnemy(Enemy other)
        {
            other.ReceiveDamage(damage);
            Destroy(gameObject);
        }

        protected virtual void OnImpactedWall()
        {
            Destroy(gameObject);
        }

        private static bool IsWall(Collider other) => other.tag.Equals("wall") || LayerMask.LayerToName(other.gameObject.layer).Equals("Track");

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * speed);
        }
    }
}