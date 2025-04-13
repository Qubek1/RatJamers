using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingMinigame : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve cuttingPointMovement;
    [SerializeField]
    private float pizzaRotationSpeed;
    [SerializeField]
    private float cuttingRotationSpeed;
    [SerializeField]
    private float minTimeBetweenInputs = 0.3f;
    [SerializeField]
    private float cuttingAnimationTime = 0.2f;
    [SerializeField]
    private float cuttingAnimationDistance = 1f;
    [SerializeField]
    private Transform cuttingPoint;
    [SerializeField]
    private Transform cuttingPointStartPos;
    [SerializeField]
    private SpriteMask spriteMask;
    [SerializeField]
    private GameObject pizzaPrefab;
    [SerializeField]
    private Transform objectsParent;
    [SerializeField]
    private float movementAfterCut = 0.1f;

    private float lastCutTime = -1;
    private float lastInputTime = -1;
    private float animationProgress = 0;
    private int startSortingLayerDiff = 50;
    private int currentSortingLayerDiff;
    private float error = 0;

    [SerializeField]
    private List<GameObject> currentObjects;

    // Start is called before the first frame update
    void Start()
    {
        currentSortingLayerDiff = startSortingLayerDiff;
        currentObjects = new List<GameObject>() { Instantiate(pizzaPrefab, transform.position, new Quaternion(0, 0, 0, 0), objectsParent) };
        error = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && Time.time > lastInputTime + minTimeBetweenInputs)
        {
            error = Mathf.Abs(cuttingPoint.position.x - cuttingPointStartPos.position.x);
            lastCutTime = Time.time;
            lastInputTime = Time.time;
            List<GameObject> newGameObjects = new List<GameObject>();
            foreach (GameObject gameObject in currentObjects)
            {
                newGameObjects.Add(Cut(gameObject, cuttingPoint));
            }
            currentObjects.AddRange(newGameObjects);
            currentSortingLayerDiff /= 2;
        }
        if (Time.time > lastCutTime + cuttingAnimationTime)
        {
            objectsParent.Rotate(Vector3.forward, pizzaRotationSpeed * Time.deltaTime);
            animationProgress += Time.deltaTime;
            if (animationProgress > cuttingPointMovement.keys[^1].time)
            {
                animationProgress -= cuttingPointMovement.keys[^1].time;
            }
            cuttingPoint.transform.position = 
                cuttingPointStartPos.position + Vector3.right * cuttingPointMovement.Evaluate(animationProgress) * cuttingAnimationDistance;
            cuttingPoint.transform.parent.Rotate(Vector3.forward, cuttingRotationSpeed * Time.deltaTime);
        }
    }

    private GameObject Cut(GameObject objectToCut, Transform cuttingPoint)
    {
        GameObject newObject = Instantiate(objectToCut, objectToCut.transform.position, objectToCut.transform.rotation, objectsParent);
        SpriteRenderer objectLowSprite = objectToCut.GetComponent<SpriteRenderer>();
        SpriteRenderer objectHighSprite = newObject.GetComponent<SpriteRenderer>();
        int currentSortingLayer = objectLowSprite.sortingOrder;
        objectLowSprite.sortingOrder = currentSortingLayer - currentSortingLayerDiff;
        objectHighSprite.sortingOrder = currentSortingLayer + currentSortingLayerDiff;

        SpriteMask newMaskLow = Instantiate(spriteMask, cuttingPoint.position, cuttingPoint.rotation, objectLowSprite.transform);
        newMaskLow.transform.Rotate(Vector3.forward, -90);
        newMaskLow.isCustomRangeActive = true;
        foreach (SpriteMask spriteMaskChild in objectToCut.GetComponentsInChildren<SpriteMask>())
        {
            spriteMaskChild.frontSortingOrder = objectLowSprite.sortingOrder + currentSortingLayerDiff;
            spriteMaskChild.backSortingOrder = objectLowSprite.sortingOrder - currentSortingLayerDiff;
        }
        
        SpriteMask newMaskHigh = Instantiate(spriteMask, cuttingPoint.position, cuttingPoint.transform.rotation, objectHighSprite.transform);
        newMaskHigh.transform.Rotate(Vector3.forward, 90);
        foreach (SpriteMask spriteMaskChild in newObject.GetComponentsInChildren<SpriteMask>())
        {
            spriteMaskChild.frontSortingOrder = objectHighSprite.sortingOrder + currentSortingLayerDiff;
            spriteMaskChild.backSortingOrder = objectHighSprite.sortingOrder - currentSortingLayerDiff;
        }

        objectLowSprite.transform.position += cuttingPoint.right * movementAfterCut;
        objectHighSprite.transform.position += -cuttingPoint.right * movementAfterCut;

        return newObject;
    }
}
