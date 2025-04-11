using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxButton : MonoBehaviour
{
    [SerializeField]
    private Sprite buttonUpSprite;
    [SerializeField]
    private Sprite buttonDownSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Press()
    {
        spriteRenderer.sprite = buttonDownSprite;
    }

    public void Release()
    {
        spriteRenderer.sprite = buttonUpSprite;
    }
}
