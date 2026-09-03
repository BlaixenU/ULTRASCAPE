using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltraScape.Enemies
{
    public class Telefragger : MonoBehaviour, IEndlessSpawnable
    {
        public float cooldownTime = 6.1f;

        public float attackTime = 0.9f; // attack with coroutine // no

        [Tooltip("Parented to player.")]
        public GameObject attackEffect = null!;

        private GameObject? attackEffectInstance = null;

        public GameObject teleportEffect = null!;

        public float minimumTeleportDistance = 20;

        private bool attacking = false;

        private float cooldownTimer = 0;

        private float attackTimer = 0;

        private List<Vector3> velocityVectors = new();

        private void Start()
        {
            attacking = false;
        }

        private void Update()
        {
            if (!NewMovement.Instance)
            {
                ResetTimers();
                return;
            }

            if (attacking)
            {
                if (attackTimer >= attackTime)
                {
                    Teleport();
                }

                attackTimer += Time.deltaTime * Time.timeScale;
            }
            else
            {
                if (cooldownTimer >= cooldownTime)
                {
                    StartAttack();
                }

                cooldownTimer += Time.deltaTime * Time.timeScale;
            }
        }

        private void FixedUpdate()
        {
            if (!NewMovement.Instance)
            {
                return;
            }

            if (attacking)
            {
                velocityVectors.Add((Vector3)(NewMovement.Instance?.rb.velocity)!);
            }
        }

        public void Teleport()
        {
            ResetTimers();

            attacking = false;

            Vector3 sumVector = new();

            foreach (Vector3 velocityVector in velocityVectors)
            {
                sumVector += velocityVector;
            }

            Vector3 averageVector = sumVector / velocityVectors.Count();

            Vector3 flattenedVector = averageVector.normalized - new Vector3(0, averageVector.normalized.y, 0);

            Vector3 destinationVector = flattenedVector * Mathf.Clamp(flattenedVector.magnitude, minimumTeleportDistance, minimumTeleportDistance * 2);

            transform.position = (Vector3)(NewMovement.Instance?.transform.position + destinationVector)!;

            velocityVectors = [];

            if (teleportEffect)
            {
                Instantiate(teleportEffect, transform);
            }

            attackEffectInstance?.transform.SetParent(transform);
        }

        public void StartAttack()
        {
            ResetTimers();

            attacking = true;

            if (attackEffect)
            {
                attackEffectInstance = Instantiate(attackEffect, NewMovement.Instance?.transform);
            }
        }

        private void ResetTimers()
        {
            cooldownTimer = 0f;
            attackTimer = 0f;
        }

        public void EndlessSpawn()
        {

        }
    }
}