using System;
using UnityEngine;
using UnityEngine.UI;

public class MainProgressBar : MonoBehaviour
{


    [SerializeField] private Slider m_UISlider;
    public ProductivityBar[] productivityBars;
    public float maxProgress = 100f;
    public float currentProgress = 0f;

    public float progressMultiplier = 1f; // np. ile procent postępu przy pełnej produktywności
    
    public event EventHandler ProgressReachedThreshhold; 
    private bool hasTriggered25 = false;

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
        
        float progressRatio = currentProgress / maxProgress;
        m_UISlider.value = progressRatio;
        
        
        if (!hasTriggered25 && progressRatio >= 0.25f)
        {
            hasTriggered25 = true;
            OnProgressReachedQuarter(EventArgs.Empty);
        }

        //float ratio = currentProgress / maxProgress;
        
        //if (Mathf.Approximately(newScaleX, 0))
        //    newScaleX = 1;
        
        //barTransform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

        //float deltaX = (initialScale.x - newScaleX) / 2f;
        //barTransform.localPosition = initialPosition - new Vector3(deltaX, 0, 0);
    }
    
    private void OnProgressReachedQuarter(EventArgs e)
    {
        ProgressReachedThreshhold?.Invoke(this, e);
    }
    
    
    
}