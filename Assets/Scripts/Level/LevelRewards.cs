using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelRewards {

    private static readonly Vector2 COIN_SPAWN_OFFSET = new Vector2(0.0f, -0.5f);
    private const float COIN_SPAWN_RADIUS = 0.4f;

    /* this reward is given if level completion time is lower than this value */
    public float TimeThreshold;

    public int NumWads;
    public int NumBloodWads;
    public int NumCoins;

	public GameObject[] Give(Vector2 rewardSpawnPoint)
    {
        List<GameObject> rewards = new List<GameObject>();

        SpawnSolidEntities<Wad>(NumWads, rewardSpawnPoint, rewards);

        GameObject coinObj;
        float angle = -Mathf.PI/2.0f;
        for (int i = 0; i < NumCoins; i++)
        {
            coinObj = EntityFactory.Instance.Create<Coin>().gameObject;
            //spawn around the point
            coinObj.transform.position = 
                rewardSpawnPoint + COIN_SPAWN_OFFSET + COIN_SPAWN_RADIUS * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            angle += 2.0f * Mathf.PI / NumCoins;

            rewards.Add(coinObj);
        }

        return rewards.ToArray();
    }

    private void SpawnSolidEntities<TEntity>(int amount, Vector2 rewardSpawnPoint, List<GameObject> rewards) where TEntity : Entity
    {
        GameObject entityObj;
        for (int i = 0; i < amount; i++)
        {
            entityObj = EntityFactory.Instance.Create<TEntity>().gameObject;
            entityObj.transform.position = rewardSpawnPoint;

            rewards.Add(entityObj);
        }
    }
}
