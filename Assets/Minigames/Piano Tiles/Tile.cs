using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve sizeAnimation;
    [SerializeField]
    private AnimationCurve alphaAnimation;
    [SerializeField]
    private SpriteRenderer sprite;

    public float time;
    public int lane;

    [NonSerialized]
    public bool readyToUse = true;

    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
    }

    public void Restart()
    {
        Color newColor = sprite.color;
        newColor.a = 1;
        sprite.color = newColor;
        transform.localScale = startScale;
    }

    public void ChangeColor(Color newColor)
    {
        sprite.color = newColor;
    }

    public void Click()
    {
        readyToUse = false;
        StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        float startTime = Time.time;
        while (startTime + alphaAnimation.keys.Last().time > Time.time)
        {
            Color newColor = sprite.color;
            newColor.a = alphaAnimation.Evaluate(Time.time - startTime);
            sprite.color = newColor;
            transform.localScale = startScale * sizeAnimation.Evaluate(Time.time - startTime);
            yield return null;
        }
        readyToUse = true;
        gameObject.SetActive(false);
    }
}
