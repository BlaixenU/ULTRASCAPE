using UnityEngine;

namespace UltraScape.Enemies
{
    public class Mart : MonoBehaviour, IEndlessSpawnable
    {
        public Collider mergeCollider = null!;

        private void Awake()
        {
            mergeCollider = GetComponentInChildren<Collider>();
        }

        public void EndlessSpawn()
        {

        }
    }
}