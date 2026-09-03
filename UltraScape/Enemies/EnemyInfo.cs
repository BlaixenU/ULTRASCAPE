using System.Collections.Generic;
using UnityEngine;

namespace UltraScape.Enemies
{
    public enum USEnemyType
    {
        none,
        baby,
        telefragger,
        icbm,
        mart, // dont add anything until im sure im adding it or i'll feel ashamed and quit working on this
    }

    public class EnemyInfo
    {
        public readonly string name;

        public readonly GameObject prefab;

        public readonly int[] startingWave;

        public readonly int[] maxStack;

        public List<GameObject> activeInstances;

        internal int spawnBuffer;

        internal EnemyInfo(string Name, GameObject gameObject, int[] startingWaves, int[] maxStacks)
        {
            name = Name;
            prefab = gameObject;
            startingWave = startingWaves;
            maxStack = maxStacks;

            activeInstances = [];
        }
    }
}