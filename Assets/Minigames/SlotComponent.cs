using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private Vector2 _initalScale;
    private void Awake()
    {
        _initalScale = transform.localScale;
    }

    public void OnSelected()
    {
        transform.localScale = _initalScale * 1.2f;
    }

    public void OnDeselected()
    {
        transform.localScale = _initalScale;
    }
}
