using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeQAGame : MonoBehaviour
{
    [SerializeField] private MainProgressBar mainProgressBar;
    private BoxCollider2D boxCollider;
    public Telewizor Telewizor;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        mainProgressBar.ProgressReachedThreshhold += OnProgressReachedThresh;
    }

    private void OnProgressReachedThresh(object sender, EventArgs e)
    {
        boxCollider.enabled = true;
        Debug.Log("🎯 BoxCollider2D został włączony!");
        Telewizor.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}