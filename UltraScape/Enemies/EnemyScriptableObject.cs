using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraScape.Enemies
{
    [CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ULTRASCAPE/EnemyScriptableObject")]
    public class EnemyScriptableObject : ScriptableObject
    {
        public string enemyName = null!;

        public GameObject prefab = null!;

        public DifficultySettings<int> startingWaves;

        public DifficultySettings<int> maxStacks;
    }

    [System.Serializable]
    public struct DifficultySettings<T>
    {
        public T casual;

        public T standard;

        public T extreme;

        public readonly T GetValue(int difficulty)
        {
            return difficulty switch
            {
                0 => casual,
                1 => standard,
                2 => extreme,
                _ => standard,
            };
        }
    }
}