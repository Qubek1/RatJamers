using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QATestMinigame : MonoBehaviour
{
    public int progressState = 0;

    [SerializeField]
    private List<ListOfGameObjects> objectsToSpawnWithProgress;
    [SerializeField]
    private List<SpriteWithProgress> spritesWithProgress;

    private void UpdateProgress()
    {
        for (int i = 0; i <= progressState; i++)
        {
            foreach (GameObject objectToSpawn in objectsToSpawnWithProgress[i].list)
            {
                objectToSpawn.SetActive(true);
            }
        }
    }
    
}

[Serializable]
public struct ListOfGameObjects
{
    public List<GameObject> list;
    public ListOfGameObjects(List<GameObject> list)
    {
        this.list = list;
    }
}