using UnityEngine;

namespace UltraScape.Enemies
{
    public class FlyingEntity : MonoBehaviour
    {
        public bool shouldDrift = false;

        public float driftRate = 0.0f;

        public AnimationCurve displacementGraph = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        public UltrakillEvent onDisplace = null!;

        private float displacementTimer;

        private float displacementDistance;

        private NewMovement player => NewMovement.Instance!;

        [HideInInspector]
        public bool isDisplacing = false;



        private void Start()
        {
            isDisplacing = false;
        }

        private void Update()
        {
            if (player && shouldDrift)
            {
                DoDrift();
            }
        }

        private void DoDrift()
        {
            transform.position = Quaternion.LookRotation(player.transform.position - transform.position) * new Vector3(0, 0, 1) * driftRate;
        }

        public void Displace() { } // write later when i need to 
    }
}