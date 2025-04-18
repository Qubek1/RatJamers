using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ClickingOrderController : MonoBehaviour
{
    public int lenght = 10;
    public float spaceBetweenButtons;
    public XboxButtonSprite xboxButtonSpritePrefab;
    public int progress = 0;
    public List<int> buttonsToClickIndexes;

    public List<XboxButtonSprite> xboxButtons;

    private void Update()
    {
        UpdateProgress();
    }
    
    public void Generate()
    {
        xboxButtons = new List<XboxButtonSprite>();
        for(int i=0; i<lenght; i++)
        {
            xboxButtons.Add(
            Instantiate(
                xboxButtonSpritePrefab,
                transform.position + new Vector3(i * spaceBetweenButtons - (spaceBetweenButtons / 2) * (lenght - 1), 0),
                new Quaternion(0, 0, 0, 0),
                transform).GetComponent<XboxButtonSprite>());
            xboxButtons[i].SetButton(buttonsToClickIndexes[i]);
        }
    }

    public void Click(int buttonIndex)
    {
        if (buttonIndex == buttonsToClickIndexes[progress])
        {
            progress++;
        }
        else
        {
            progress = 0;
        }
    }

    private void UpdateProgress()
    {
        for(int i=0; i<xboxButtons.Count; i++)
        {
            if (i < progress)
            {
                xboxButtons[i].SetToFull();
            }
            else
            {
                xboxButtons[i].SetToHollow();
            }
        }
    }

    public void Restart()
    {
        foreach (XboxButtonSprite xboxButtonSprite in xboxButtons)
        {
            Destroy(xboxButtonSprite.gameObject);
        }
        xboxButtons.Clear();
        progress = 0;
    }

    public bool IsComplete()
    {
        return progress == lenght;
    }
}
