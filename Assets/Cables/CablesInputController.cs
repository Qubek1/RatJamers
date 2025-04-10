using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablesInputController : MonoBehaviour
{
    public List<Cable> cables;
    public int currentlyControlledCableIndex;
    public CablesOverlapController overlapController;

    // Start is called before the first frame update
    void Start()
    {
        overlapController.interactedCable = cables[currentlyControlledCableIndex];
        for (int cableIndex = 0; cableIndex < cables.Count; cableIndex++)
        {
            if (cableIndex == currentlyControlledCableIndex)
            {
                cables[cableIndex].Select();
            }
            else
            {
                cables[cableIndex].Deselect();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool changed = false;
        if (Input.GetKey(KeyCode.Alpha1))
        {
            changed = true;
            currentlyControlledCableIndex = 0;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            changed = true;
            currentlyControlledCableIndex = 1;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            changed = true;
            currentlyControlledCableIndex = 2;
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            changed = true;
            currentlyControlledCableIndex = 3;
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            changed = true;
            currentlyControlledCableIndex = 4;
        }
        if (changed)
        {
            overlapController.interactedCable = cables[currentlyControlledCableIndex];
            for (int cableIndex = 0; cableIndex < cables.Count; cableIndex++)
            {
                if (cableIndex == currentlyControlledCableIndex)
                {
                    cables[cableIndex].Select();
                }
                else
                {
                    cables[cableIndex].Deselect();
                }
            }
        }
        cables[currentlyControlledCableIndex].SetMovementVector(Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up);
    }
}
