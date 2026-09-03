using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UltraScape.Enemies
{
    public class FlyingEnemy : MonoBehaviour
    {
        // dont forget that for body sound loop you can just add an audio source child xd

        [Header("Drift")]
        public bool drift = true;

        public float driftRate = 0.0f;

        [Space(10f)]

        [Header("Displacement")]
        public float displacementDistance;

        public float displacementTime;

        public AnimationCurve displacementGraph = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [Space(5f)]
        public UnityEvent onDisplacementStart = null!;
        public UnityEvent onDisplacementEnd = null!;

        [HideInInspector]
        public bool displacing = false;

        private void Awake()
        {
            displacing = false;
        }

        private void Update()
        {
            if (!NewMovement.Instance) return;

            if (drift && driftRate != 0 && !displacing)
            {
                Drift(NewMovement.Instance!.transform.position, driftRate * Time.deltaTime * Time.timeScale);
            }
        }

        private void Drift(Vector3 targetPosition, float magnitude)
        {
            transform.position += Quaternion.LookRotation(targetPosition - transform.position) * Vector3.forward * magnitude;
        }

        public void Displace(Vector3 destination)
        {
            if (displacing) return;

            StartCoroutine(DisplaceRoutine(destination));
        }

        private IEnumerator DisplaceRoutine(Vector3 destination)
        {
            displacing = true;

            float totalTime = 0;

            Vector3 startPos = transform.position;

            Vector3 travelVector = destination - startPos;

            onDisplacementStart.Invoke();

            while (totalTime < displacementTime)
            {
                var timeNormalized = totalTime / displacementTime;

                transform.position = startPos + (displacementGraph.Evaluate(timeNormalized) * travelVector);

                yield return new WaitForSeconds(Time.fixedDeltaTime);
                totalTime += Time.fixedDeltaTime;
            }

            displacing = false;

            onDisplacementEnd.Invoke();
        }
    }
}