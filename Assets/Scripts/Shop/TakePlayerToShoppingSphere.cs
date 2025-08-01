using UnityEngine;

namespace Shop
{
    public class TakePlayerToShoppingSphere : MonoBehaviour
    {
        public Objective objective;
        
        void Start()
        {
            objective.OnLapCompleted += OnLapCompleted;
        }

        private void OnDestroy()
        {
            if (objective)
            {
                objective.OnLapCompleted -= OnLapCompleted;
            }
        }

        private void OnLapCompleted()
        {
            ShoppingSphere.Instance.Activate(gameObject);
        }
    }
}
