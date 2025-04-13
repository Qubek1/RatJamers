using UnityEngine;

public class ProductivityBar : MonoBehaviour {

    public float maxProductivity = 100f;
    public float currentProductivity;
    public float drainRate = 5f;

    private Transform barTransform;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    private SpriteRenderer spriteRenderer;
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;

    public float blinkThreshold = 0.4f;
    public float blinkSpeed = 4f;

    private bool isBlinking = false;
    private bool wasBlinking = false;


    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        barTransform = transform;
        initialScale = barTransform.localScale;
        initialPosition = barTransform.localPosition;
        currentProductivity = maxProductivity;
        MinigamesManager.MinigameLeftAction += HandleMinigameEnd;
    }

    private void HandleMinigameEnd(int playerLeft)
    {
        
    }

    void Update() {
        currentProductivity -= drainRate * Time.deltaTime;
        currentProductivity = Mathf.Clamp(currentProductivity, 0, maxProductivity);

        float ratio = currentProductivity / maxProductivity;
        float newScaleX = initialScale.x * ratio;
        barTransform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

        float deltaX = (initialScale.x - newScaleX) / 2f;
        barTransform.localPosition = initialPosition - new Vector3(deltaX, 0, 0);

        UpdateBarColor();
        if (isBlinking) {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            Color blinkColor = lowColor;
            blinkColor.a = alpha;
            spriteRenderer.color = blinkColor;
        }


        // Testng
        // if (currentProductivity < 50) {
        //     UpdateCurrentProductivityOnMiniGameEnd(50f);
        // }
    }

    public void UpdateCurrentProductivityOnMiniGameEnd(float updateAmount) {
        currentProductivity += updateAmount;
        currentProductivity = Mathf.Clamp(currentProductivity, 0f, maxProductivity);

        float ratio = currentProductivity / maxProductivity;
        float newScaleX = initialScale.x * ratio;
        barTransform.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);

        float deltaX = (initialScale.x - newScaleX) / 2f;
        barTransform.localPosition = initialPosition - new Vector3(deltaX, 0, 0);

        UpdateBarColor();
    }

    private void UpdateBarColor() {
        float ratio = currentProductivity / maxProductivity;

        // Czy powinien migać?
        isBlinking = (ratio < blinkThreshold);

        if (!isBlinking) {
            // Płynna zmiana koloru
            if (ratio > 0.5f) {
                float t = (ratio - 0.5f) * 2f;
                spriteRenderer.color = Color.Lerp(midColor, fullColor, t);
            }
            else {
                float t = ratio * 2f;
                spriteRenderer.color = Color.Lerp(lowColor, midColor, t);
            }
        }
    }

    public float GetRatio() {
        return currentProductivity / maxProductivity;
    }
}