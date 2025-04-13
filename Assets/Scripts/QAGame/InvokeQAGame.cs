using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeQAGame : MonoBehaviour
{
    [SerializeField] private MainProgressBar mainProgressBar;
    public Telewizor Telewizor;
    public WorkstationController WorkstationController;

    // Start is called before the first frame update
    void Start()
    {
        mainProgressBar.ProgressReachedThreshhold += OnProgressReachedThresh;
    }

    private void OnProgressReachedThresh(object sender, EventArgs e)
    {
        Debug.Log("🎯 BoxCollider2D został włączony!");
        Telewizor.gameObject.SetActive(true);
        WorkstationController.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
// Kuba TUTAJ 
    }
}