using System.Collections;
using UnityEngine;

public class RingPulse : MonoBehaviour
{
    public void Play(float targetRadius, float duration)
    {
        StartCoroutine(PulseRoutine(targetRadius, duration));
    }

    private IEnumerator PulseRoutine(float targetRadius, float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;

        // Figure out scale needed: sprite is X units wide at scale 1, we want diameter = targetRadius*2
        float targetScale = (targetRadius * 2f) / sr.bounds.size.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.one * Mathf.Lerp(0f, targetScale, t);
            Color c = sr.color;
            c.a = 1f - t;
            sr.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}