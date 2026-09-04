using UnityEngine;

namespace UltraScape.Enemies
{
    public class Mart : MonoBehaviour, ICustomEndlessSpawnable
    {
        public Collider mergeCollider = null!;

        private void Awake()
        {
            mergeCollider = GetComponentInChildren<Collider>();
        }

        public void AfterSpawn()
        {

        }
    }
}