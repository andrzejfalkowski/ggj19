using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float initialDelay = 0f;
    [SerializeField]
    private float minDelay = 3f;
    [SerializeField]
    private float maxDelay = 5f;

    [SerializeField]
    private int limit = 1;
    private int currentSpawns = 0;

    [SerializeField]
    private List<GameObject> prefabsToSpawn;

    private bool initial = true;

    private void Start()
    {
        SpawnNext();
    }

    void SpawnNext()
    {
        DOVirtual.DelayedCall(initial ? initialDelay : UnityEngine.Random.Range(minDelay, maxDelay), () => 
        {
            if (currentSpawns < limit)
            {
                GameObject prefab = prefabsToSpawn[UnityEngine.Random.Range(0, prefabsToSpawn.Count)];
                GameObject spawned = GameObject.Instantiate(prefab, this.transform);
                spawned.transform.localPosition = Vector3.zero;
                spawned.transform.localScale = Vector3.one;

                if (spawned.GetComponent<Interactable>() != null)
                {
                    spawned.GetComponent<Interactable>().Init(this);
                }

                currentSpawns++;
            }

            if (currentSpawns < limit)
            {
                SpawnNext();
            }
        });

        initial = false;
    }

    public void HandleSpawnDestruction()
    {
        currentSpawns--;
        if (currentSpawns < limit)
        {
            SpawnNext();
        }
    }
}
