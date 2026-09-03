using System;
using UnityEngine;

namespace UltraScape.Enemies
{
    [Obsolete]
    public readonly struct EndlessSpawningData
    {
        public readonly GameObject prefab;

        public readonly int[] startingWave;

        public readonly int[] maxStack;

        internal EndlessSpawningData(GameObject gameObject, int[] startingWaves, int[] maxStacks)
        {
            prefab = gameObject;
            startingWave = startingWaves;
            maxStack = maxStacks;
        }
    }
}