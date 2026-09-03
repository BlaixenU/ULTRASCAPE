using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

namespace UltraScape.Enemies
{
    [ConfigureSingleton(SingletonFlags.NoAutoInstance)]
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        private bool _endlessEnabled;

        public bool EndlessEnabled
        {
            get
            {
                return _endlessEnabled;
            }
            internal set
            {
                _endlessEnabled = value;
            }
        }

        private readonly int spawnQuantity = 2;

        private readonly int spawnGap = 5;

        private readonly float spawnDelayIncrement = 1f;

        private bool InEndless => SceneHelper.CurrentScene == "Endless";

        private int timesSpawned;

        private Queue<USEnemyType> enemyQueue = new();

        public static int NSDiff => PrefsManager.Instance?.GetInt("difficulty") switch
        {
            > 3 => 2,
            > 1 => 1,
            _ => 0
        };

        /* [Obsolete]
        public static readonly Dictionary<USEnemyType, EndlessSpawningData> spawningData = new()
        {
            [USEnemyType.baby] = new EndlessSpawningData(Assetloader.Baby, new int[] { 1, 1, 1 }, new int[] { 4, 4, 8 }),
            [USEnemyType.telefragger] = new EndlessSpawningData(Assetloader.Telefragger, new int[] { 1, 1, 1 }, new int[] { 2, 2, 4 })
        };

        [Obsolete]
        public Dictionary<USEnemyType, List<GameObject>> activeEnemies = new()
        {
            [USEnemyType.baby] = new(),
            [USEnemyType.telefragger] = new()
        }; */

        public static Dictionary<USEnemyType, EnemyInfo> enemies = new()
        {
            [USEnemyType.baby] = new EnemyInfo("Baby", Assetloader.Baby, [1, 1, 1], [4, 4, 8]),
            [USEnemyType.telefragger] = new EnemyInfo("Telefragger", Assetloader.Telefragger, [1, 1, 1], [2, 2, 4])
        };

        private void Awake()
        {
            EndlessEnabled = true;

            if (InEndless)
            {
                timesSpawned = 0;
            }

            gameObject.transform.position = new(0, 0, 0);

            foreach (KeyValuePair<USEnemyType, EnemyInfo> kvp in enemies)
            {
                kvp.value.spawnBuffer = 0;
            }
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
                List<USEnemyType> enemyPool = [];

                foreach (KeyValuePair<USEnemyType, EnemyInfo> kvp in enemies)
                {
                    if ((batchWave >= kvp.value.startingWave[NSDiff]) && (enemies[kvp.key].activeInstances.Count + enemies[kvp.key].spawnBuffer) < kvp.value.maxStack[NSDiff])
                    {
                        enemyPool.Add(kvp.key);

                        enemies[kvp.key].spawnBuffer++;

                        Plugin.Log($"{kvp.value.name} added to enemy pool.");
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
                USEnemyType nextEnemy = enemyQueue.Dequeue();

                SpawnEnemy(nextEnemy);

                enemies[nextEnemy].spawnBuffer--;

                yield return new WaitForSeconds(spawnDelayIncrement);
            }
        }

        private void SpawnEnemy(USEnemyType enemyType)
        {
            GameObject enemyObject = enemies[enemyType].prefab;

            GameObject enemyInstance = Instantiate(enemyObject, gameObject.transform);

            TypeInfo typeInfo = typeof(enemyInstance.GetComponent<Enemy>()).GetTypeInfo();

            // USE CUSTOM ATTRIBUTES TO DETERMINE ENEMY SPAWN TYPE. FOR NEW TYPES ADD LOGIC TO HERE. NO ITS NOT SPAGHETTI CODE SHUT YOUR ASS

        }


        public void AddEnemy(GameObject enemy, USEnemyType type)
        {
            enemies[type].activeInstances.Add(enemy);
        }

        public void RemoveEnemy(GameObject enemy, USEnemyType type)
        {
            for (int i = 0; i < enemies[type].activeInstances.Count; i++)
            {
                if (enemies[type].activeInstances[i] == enemy)
                {
                    enemies[type].activeInstances.RemoveAt(i);
                    return;
                }
            }
        }
    }
}