using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UltraScape.Enemies
{
    public class AerialBody : MonoBehaviour
    {
        // dont forget that for body sound loop you can just add an audio source child xd

        [Header("Drift")]
        public bool drift = true;

        public float driftRate = 0.0f;

        [Space(5f)]
        public bool targetPlayer = true;

        public Transform target = null!;

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

        public Transform GetTarget
        {
            get
            {
                if (targetPlayer) return NewMovement.Instance!.transform;
                if (target != null) return target;
                return null!;
            }
        }

        private void Awake()
        {
            displacing = false;
        }

        private void Update()
        {
            if (!NewMovement.Instance) return;

            if (drift && driftRate != 0 && !displacing)
            {
                Drift(GetTarget.position, driftRate * Time.deltaTime * Time.timeScale);
            }
        }

        private void Drift(Vector3 targetPosition, float magnitude)
        {
            transform.position += Quaternion.LookRotation(targetPosition - transform.position) * Vector3.forward * magnitude;
        }

        public void Displace(Vector3 destination)
        {
            StopCoroutine(nameof(DisplaceRoutine));

            StartCoroutine(DisplaceRoutine(destination, displacementTime));
        }

        private IEnumerator DisplaceRoutine(Vector3 destination, float duration)
        {
            displacing = true;

            float totalTime = 0;

            Vector3 startPos = transform.position;

            Vector3 travelVector = destination - startPos;

            onDisplacementStart.Invoke();

            while (totalTime < duration)
            {
                var timeNormalized = totalTime / duration;

                transform.position = startPos + (displacementGraph.Evaluate(timeNormalized) * travelVector);

                yield return new WaitForSeconds(Time.fixedDeltaTime);
                totalTime += Time.fixedDeltaTime;
            }

            displacing = false;

            onDisplacementEnd.Invoke();
        }
    }
}