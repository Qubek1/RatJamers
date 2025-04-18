using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteWithProgress : MonoBehaviour
{
    [SerializeField]
    private Sprite placeHolder;
    [SerializeField]
    private Sprite normalAsset;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPlaceHolder()
    {
        spriteRenderer.sprite = placeHolder;
    }

    public void SetNormalAsset()
    {
        spriteRenderer.sprite = normalAsset;
    }
}
