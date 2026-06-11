using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    [SerializeField] private float bounceHeight = 0.3f;
    [SerializeField] private float bounceDuration = 0.4f;
    [SerializeField] private int bounceCount = 2;

    public void StartBounce()
    {
        StartCoroutine(BounceHandler());
    }

    private IEnumerator BounceHandler()
    {
        Vector3 startPosition = transform.position;
        float localHeight = bounceHeight;
        float localDuration = bounceDuration;

        for(int i = 0; i < bounceCount; i ++)
        {
            yield return Bounce(startPosition, localHeight, localDuration / 2); // 单边持续时间除以2
            localHeight *= 0.5f;
            localDuration *= 0.8f;
        }

        transform.position = startPosition;
    }

    private IEnumerator Bounce(Vector3 start, float height, float duration)
    {
        Vector3 peak = start + Vector3.up * height;
        float elapsed = 0f; // 时间流逝了多少

        // 向上
        while(elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, peak, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // 向下
        while(elapsed < duration)
        {
            transform.position = Vector3.Lerp(peak, start, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
