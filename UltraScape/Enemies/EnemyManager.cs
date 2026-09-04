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

        private int timesSpawned;

        private Queue<EnemyScriptableObject> enemyQueue = new();

        public static int NSDiff => PrefsManager.Instance?.GetInt("difficulty") switch
        {
            > 3 => 2,
            > 1 => 1,
            _ => 0
        };

        [NonSerialized]
        public Dictionary<EnemyScriptableObject, EnemySceneInfo> enemies = [];

        private void Awake()
        {
            if (InEndless)
            {
                timesSpawned = 0;
            }

            gameObject.transform.position = Vector3.zero;
        }

        internal void WaveChange(int newWave)
        {
            if (timesSpawned == 0)
            {
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

        private void QueueBatch(int batchWave)
        {
            for (int i = 0; i < spawnQuantity; i++)
            {
                List<EnemyScriptableObject> enemyPool = [];

                foreach (KeyValuePair<EnemyScriptableObject, EnemySceneInfo> kvp in enemies)
                {
                    if ((batchWave >= kvp.Key.startingWaves.GetValue(NSDiff)) && (enemies[kvp.Key].activeInstances.Count + enemies[kvp.Key].spawnBuffer) < kvp.Key.maxStacks.GetValue(NSDiff))
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