using UnityEngine;
using TMPro;
public class FpsCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsTxt;
    private int frameCount = 0;
    private float elapsedTime = 0f;
    private float fps = 0f;

    private void Awake()
    {
        fpsTxt = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= 1f)
        {
            fps = frameCount / elapsedTime;
            fpsTxt.text = "FPS: " + Mathf.RoundToInt(fps);

            frameCount = 0;
            elapsedTime = 0f;
        }
    }
}
