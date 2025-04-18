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
    private int _buttonIndex;
    [SerializeField]
    private bool _full;
    private SpriteRenderer spriteRenderer;

    public int buttonIndex => _buttonIndex;
    public bool full => _full;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void SetButton(int buttonIndex)
    {
        this._buttonIndex = buttonIndex;
        UpdateSprite();
    }

    public void SetToFull()
    {
        _full = true;
        UpdateSprite();
    }

    public void SetToHollow()
    {
        _full = false;
        UpdateSprite();
    }

    public void FlipFillState()
    {
        _full = !_full;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (_full)
        {
            spriteRenderer.sprite = fullSprites[_buttonIndex];
        }
        else
        {
            spriteRenderer.sprite = hollowSprites[_buttonIndex];
        }
    }
}
