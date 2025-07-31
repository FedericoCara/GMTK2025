using UnityEngine;

namespace Shop
{
    public class TakePlayerToShoppingSphere : MonoBehaviour
    {
        public LapObject endingLap;
        
        void Start()
        {
            endingLap.OnLapCompleted += OnLapCompleted;
        }

        private void OnLapCompleted()
        {
            ShoppingSphere.Instance.Activate(gameObject);
        }
    }
}
