using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxButtonSprite : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> fullSprites;
    [SerializeField]
    private List<Sprite> hollowSprites;
    [SerializeField]
    private int buttonIndex;
    [SerializeField]
    private bool full;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void SetButton(int buttonIndex)
    {
        this.buttonIndex = buttonIndex;
        UpdateSprite();
    }

    public void SetToFull()
    {
        full = true;
        UpdateSprite();
    }

    public void SetToHollow()
    {
        full = false;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (full)
        {
            spriteRenderer.sprite = fullSprites[buttonIndex];
        }
        else
        {
            spriteRenderer.sprite = hollowSprites[buttonIndex];
        }
    }
}
