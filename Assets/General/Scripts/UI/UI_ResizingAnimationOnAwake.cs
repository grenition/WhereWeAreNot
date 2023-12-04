using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_ResizingAnimationOnAwake : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.2f;
    private CanvasGroup group;

    private bool startSizeSelcted = false;
    private Vector3 startSize = Vector3.one;

    [SerializeField] private bool saveStartSize = true;

    private void OnEnable()
    {
        group = GetComponent<CanvasGroup>();
        if (!startSizeSelcted && saveStartSize)
        {
            startSize = transform.localScale;
            startSizeSelcted = true;
        }
        transform.localScale = Vector3.one * 0.001f;
        group.alpha = 0f;
        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(1f, startSize, transitionTime));
    }
    private IEnumerator TransitionEnumerator(float targetAlpha, Vector3 targetSize, float transitionTime)
    {
        if (transitionTime <= 0f)
            yield break;

        float t = 0f;
        float startTime = Time.realtimeSinceStartup;
        float startAlpha = group.alpha;
        while (t < 1f)
        {
            t = (Time.realtimeSinceStartup - startTime) / transitionTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, t);
            yield return null;
        }

        if (group.alpha == 0f)
            gameObject.SetActive(false);
    }


    public void CloseWithAnimation()
    {
        if (!gameObject.activeSelf)
            return;
        //AudioController.PlayButtonSound();
        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(0f, Vector3.one * 0.001f, transitionTime));
    }
}
