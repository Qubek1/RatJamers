using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttableComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    
    public void OnSelected()
    {
        m_SpriteRenderer.color = GetColorWithAlpha(m_SpriteRenderer.color, 1.0f);
    }

    public void OnDeselected()
    {
        m_SpriteRenderer.color = GetColorWithAlpha(m_SpriteRenderer.color, 0.5f);
    }

    public void OnAddIntoSlot()
    {
        m_SpriteRenderer.enabled = false;
    }

    public void OnRemoveFromSlot()
    {
        
        m_SpriteRenderer.enabled = true;
    }

    private static Color GetColorWithAlpha(Color color, float newAlpha)
        => new(color.r, color.g, color.b, newAlpha);
}
