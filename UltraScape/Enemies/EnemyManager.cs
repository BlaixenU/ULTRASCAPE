using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace UltraScape.Enemies
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        public bool endlessEnabled;

        public int spawnQuantity = 2;

        public int spawnGap = 5;

        public float spawnDelayIncrement = 1f;

        private bool InEndless => SceneHelper.CurrentScene == "Endless";

        private int timesSpawned = 0;

        private Queue<EnemyScriptableObject> enemyQueue = new();

        public static int NSDiff => PrefsManager.Instance?.GetInt("difficulty") switch
        {
            > 3 => 2,
            > 1 => 1,
            _ => 0
        };

        public Dictionary<EnemyScriptableObject, EnemySceneInfo> enemies = [];

        private void Awake()
        {
            if (InEndless)
            {
                timesSpawned = 0;
            }

            gameObject.transform.position = Vector3.zero;
        }

        private void Start()
        {
            foreach (var kvp in enemies)
            {
                Plugin.Log($"{kvp.Key.name}");
                Plugin.Log($"{kvp.Key.prefab}");
                var startingWaves = kvp.Key.startingWaves;
                Plugin.Log($"{startingWaves.GetValue(0)}, {startingWaves.GetValue(1)}, {startingWaves.GetValue(2)}");
                var maxStacks = kvp.Key.maxStacks;
                Plugin.Log($"{maxStacks.GetValue(0)}, {maxStacks.GetValue(1)}, {maxStacks.GetValue(2)}");
                Plugin.Log("");
            }
        }

        internal void WaveChange(int newWave)
        {
            if (timesSpawned == 0)
            {
                Plugin.Log("Skipped waves detected, attempting to queue.");

                for (int i = spawnGap; i < newWave - 1; i += spawnGap)
                {
                    QueueBatch(i);
                }
            }

            if (newWave % spawnGap == 0)
            {
                QueueBatch(newWave);
            }

            StartCoroutine(SpawnQueue());
        }

        private void QueueBatch(int waveForQueue)
        {
            Plugin.Log($"Queueing for Wave {waveForQueue}");

            for (int i = 0; i < spawnQuantity; i++)
            {
                List<EnemyScriptableObject> enemyPool = [];

                foreach (KeyValuePair<EnemyScriptableObject, EnemySceneInfo> kvp in enemies)
                {
                    int startingWave = kvp.Key.startingWaves.GetValue(NSDiff);

                    int maxStack = kvp.Key.maxStacks.GetValue(NSDiff);

                    int activeInstances = kvp.Value.activeInstances.Count;

                    int spawnBuffer = kvp.Value.spawnBuffer;

                    Plugin.Log($"startingWave: {startingWave}");
                    Plugin.Log($"maxStack: {maxStack}");
                    Plugin.Log($"activeInstances: {activeInstances}");
                    Plugin.Log($"spawnBuffer: {spawnBuffer}");

                    if ((waveForQueue >= startingWave) && (activeInstances + spawnBuffer) < maxStack)
                    {
                        enemyPool.Add(kvp.Key);

                        enemies[kvp.Key].spawnBuffer++;

                        Plugin.Log($"{kvp.Key.name} added to enemy pool.");
                    }
                }

                if (enemyPool.Count > 0)
                {
                    Plugin.Log($"enemy pool consists of {enemyPool.Count} elements");

                    var index = UnityEngine.Random.Range(0, enemyPool.Count);

                    enemyQueue.Enqueue(enemyPool[index]);
                }
            }

            timesSpawned++;
        }

        private IEnumerator SpawnQueue()
        {
            Plugin.Log("Spawning queue");

            while (enemyQueue.Count > 0)
            {
                var nextEnemy = enemyQueue.Dequeue();

                SpawnEnemy(nextEnemy);

                enemies[nextEnemy].spawnBuffer--;

                yield return new WaitForSeconds(spawnDelayIncrement);
            }
        }

        private void SpawnEnemy(EnemyScriptableObject enemyScriptableObject)
        {
            Plugin.Log($"Spawning enemy: {enemyScriptableObject.name}");

            GameObject enemyInstance = Instantiate(enemyScriptableObject.prefab, gameObject.transform);

            if (endlessEnabled && InEndless && enemyInstance.TryGetComponent(out ICustomEndlessSpawnable spawnable))
            {
                spawnable.AfterSpawn();
            }
        }

        public void AddEnemy(GameObject self, EnemyScriptableObject enemyScriptableObject)
        {
            enemies[enemyScriptableObject].activeInstances.Add(self);
        }

        public void RemoveEnemy(GameObject self, EnemyScriptableObject enemyScriptableObject)
        {
            for (int i = 0; i < enemies[enemyScriptableObject].activeInstances.Count; i++)
            {
                if (enemies[enemyScriptableObject].activeInstances[i] == self)
                {
                    enemies[enemyScriptableObject].activeInstances.RemoveAt(i);
                    return;
                }
            }
        }
    }
}