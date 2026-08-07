using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    int frames = 0;
    float totalTime = 0f;
    const int windowSize = 60;

    float minFPS = float.MaxValue;
    float maxFPS = float.MinValue;

    void Update()
    {
        if (frames < windowSize)
        {
            frames++;
            totalTime += Time.deltaTime;
            return;
        }

        float currentFPS = 1f / (totalTime / windowSize);
        if (currentFPS < minFPS)
        {
            minFPS = currentFPS;
        }

        if (currentFPS > maxFPS)
        {
            maxFPS = currentFPS;
        }

        Debug.Log($"FPS: {currentFPS} | MIN: {minFPS} | MAX: {maxFPS}");

        totalTime = 0f;
        frames = 0;
    }
}
