using System;
using UnityEngine;

public class MainProgressBar : MonoBehaviour
{
    public ProductivityBar[] productivityBars;
    public float maxProgress = 100f;
    public float currentProgress = 0f;

    public float progressMultiplier = 1f; // np. ile procent postępu przy pełnej produktywności

    private Transform barTransform;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    
    private bool hasTriggered25 = false;
    
    public event EventHandler QAeventPlatformMiniGame;

    void Start()
    {
        barTransform = transform;
        initialScale = barTransform.localScale;
        initialPosition = barTransform.localPosition;
    }

    void Update()
    {
        float totalRatio = 0f;

        foreach (var bar in productivityBars)
        {
            totalRatio += bar.GetRatio(); // GetRatio() = produktywność od 0 do 1
        }

        float avgRatio = totalRatio / productivityBars.Length;

        currentProgress += avgRatio * progressMultiplier * Time.deltaTime;
        currentProgress = Mathf.Clamp(currentProgress, 0, maxProgress);

        float ratio = currentProgress / maxProgress;
        float newScaleX = initialScale.x * ratio;
        //if (Mathf.Approximately(newScaleX, 0))
        //    newScaleX = 1;
        barTransform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

        float deltaX = (initialScale.x - newScaleX) / 2f;
        barTransform.localPosition = initialPosition - new Vector3(deltaX, 0, 0);
        
        
        if (!hasTriggered25 && currentProgress >= 25f)
        {
            hasTriggered25 = true;
            OnProgressReachedQuarter(EventArgs.Empty);
        }
    }
    
    protected virtual void OnProgressReachedQuarter(EventArgs e)
    {
        QAeventPlatformMiniGame?.Invoke(this, e);
    }
}