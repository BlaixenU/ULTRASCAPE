using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UltraScape.Enemies
{
    public enum ICBMState
    {
        cooldown,
        tracking,
        falling
    }

    public class ICBM : MonoBehaviour
    {
        public GameObject fullSound = null!;

        [Space(5f)]
        public float trackingDuration;

        [Tooltip("ONLY the interval of the lockin to the explosion.")]
        public float fallingDuration;

        [Space(10f)]
        [Header("Indicator")]
        [Tooltip("In Degrees.")]
        public float rotationSpeed;

        public float scaleGrowAmount = 1.25f;

        [Space(10f)]
        [Header("The Actual ICBM")]
        public GameObject missile = null!;

        public GameObject missileInstance = null!;

        public GameObject explosion = null!;

        public float startHeight;

        [Space(20f)]
        public float cooldown;

        private float timeSpentCooldown;

        private float timeSpentTracking;

        private float timeSpentFalling;

        private Vector3 actualScale;

        private Color actualColor;

        private MaterialPropertyBlock propBlock = null!;

        private SpriteRenderer spr = null!;

        private static readonly int colorPropertyID = Shader.PropertyToID("_Color");

        [HideInInspector]
        public ICBMState State { get; private set; }

        private void Awake()
        {
            spr = GetComponent<SpriteRenderer>();

            propBlock = new();

            actualScale = transform.localScale;
            actualColor = spr.sharedMaterial.GetColor(colorPropertyID);
        }

        private void Start()
        {
            StartCoroutine(Cooldown(2 * cooldown));
        }

        private void Update()
        {
            switch (State)
            {
                case ICBMState.cooldown:

                    timeSpentCooldown += Time.deltaTime * Time.timeScale;

                    break;

                case ICBMState.tracking:

                    timeSpentTracking += Time.deltaTime * Time.timeScale;

                    transform.Rotate(Vector3.up, rotationSpeed / 360 * Time.timeScale);

                    float fadeInTime = 0.5f;

                    if (timeSpentTracking! > fadeInTime)
                    {
                        spr.GetPropertyBlock(propBlock);

                        propBlock.SetColor(colorPropertyID, new(actualColor.r, actualColor.g, actualColor.b, timeSpentTracking / fadeInTime));

                        spr.SetPropertyBlock(propBlock);

                        transform.localScale = actualScale * (1 + (fadeInTime - timeSpentTracking));
                    }

                    transform.position = NewMovement.Instance!.transform.position;

                    break;

                case ICBMState.falling:

                    timeSpentFalling += Time.deltaTime * Time.timeScale;

                    missileInstance?.transform.Translate(new(0, -startHeight / fallingDuration * Time.deltaTime * Time.timeScale, 0));

                    break;

            }
        }

        private IEnumerator Cooldown(float duration)
        {
            StateChange(ICBMState.cooldown);

            missileInstance = null!;

            yield return new WaitForSeconds(duration);


            StartCoroutine(Track(trackingDuration));
        }

        private IEnumerator Track(float tracktime)
        {
            StateChange(ICBMState.tracking);

            Instantiate(fullSound, transform);

            yield return new WaitForSeconds(tracktime);

            StartCoroutine(Fall(fallingDuration));
        }

        private IEnumerator Fall(float falltime)
        {
            StateChange(ICBMState.falling);

            missileInstance = Instantiate(missile);

            missileInstance.transform.position = transform.position + new Vector3(0, startHeight, 0);

            yield return new WaitForSeconds(falltime);


            // Explode

            StartCoroutine(Cooldown(cooldown));
        }

        private void ResetTimers()
        {
            timeSpentFalling = 0;
            timeSpentCooldown = 0;
            timeSpentTracking = 0;
        }

        private void StateChange(ICBMState newState)
        {
            ResetTimers();
            State = newState;
        }
    }
}