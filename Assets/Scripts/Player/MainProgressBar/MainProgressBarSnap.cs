using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainProgressBarSnap : MonoBehaviour
{
    [SerializeField]
    private int pointToWin = 100;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Transform plusOneSpawnPoint;
    [SerializeField]
    private float plusOnePosDif;
    [SerializeField]
    private float plusOneSpawnDelay;
    [SerializeField]
    private GameObject plusOnePrfab;
    [SerializeField]
    private int currentPoints = 0;

    private float lastSpawnTime;
    private int pointsToSpawn;

    public Action OnProgressFull;

    public void AddPoints(int p)
    {
        currentPoints += p;
        slider.value = ((float)currentPoints / pointToWin);
        pointsToSpawn += p;
        if (currentPoints >= pointToWin)
        {
            OnProgressFull?.Invoke();
        }
    }

    private void Awake()
    {
        slider.value = 0;
    }

    private void Update()
    {
        if (pointsToSpawn > 0 && lastSpawnTime + plusOneSpawnDelay < Time.time)
        {
            SpawnPlusOne();
        }
    }

    private void SpawnPlusOne()
    {
        lastSpawnTime = Time.time;
        pointsToSpawn--;
        Instantiate(
                plusOnePrfab,
                plusOneSpawnPoint.position + new Vector3(GetRandom(), GetRandom()),
                Quaternion.identity,
                plusOneSpawnPoint
                );
    }

    private float GetRandom() => UnityEngine.Random.Range(-plusOnePosDif, plusOnePosDif);
}
