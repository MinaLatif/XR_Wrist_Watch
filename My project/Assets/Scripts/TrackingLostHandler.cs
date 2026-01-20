using System.Collections;
using UnityEngine;

/// <summary>
/// Handles visual feedback when hand tracking is lost or acquired.
/// Also handles palm-up visibility for VR mode.
/// Fades the watch UI and shows a fallback message when appropriate.
/// </summary>
public class TrackingLostHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject watchVisuals;
    [SerializeField] private GameObject trackingLostPanel;

    [Header("Fade Settings")]
    [Tooltip("Duration of fade transitions in seconds")]
    [SerializeField] private float fadeDuration = 0.3f;

    [Tooltip("Faster fade for palm gesture transitions")]
    [SerializeField] private float palmGestureFadeDuration = 0.15f;

    [Header("Debounce")]
    [Tooltip("Minimum time before responding to tracking state changes (prevents flickering)")]
    [SerializeField] private float debounceTime = 0.15f;

    [Tooltip("Debounce time for palm gesture changes (shorter for responsiveness)")]
    [SerializeField] private float palmGestureDebounceTime = 0.1f;

    private CanvasGroup watchCanvasGroup;
    private CanvasGroup lostPanelCanvasGroup;
    private Coroutine fadeCoroutine;
    private Coroutine debounceCoroutine;
    private Coroutine palmDebounceCoroutine;
    private bool isTrackingLost;
    private bool isWatchHiddenByPalm;

    private void Awake()
    {
        SetupCanvasGroups();

        // Start with watch hidden until tracking is acquired
        if (watchCanvasGroup != null)
        {
            watchCanvasGroup.alpha = 0f;
        }

        if (trackingLostPanel != null)
        {
            trackingLostPanel.SetActive(true);
            if (lostPanelCanvasGroup != null)
            {
                lostPanelCanvasGroup.alpha = 1f;
            }
        }

        isTrackingLost = true;
    }

    private void SetupCanvasGroups()
    {
        // Ensure CanvasGroup exists on watch visuals
        if (watchVisuals != null)
        {
            watchCanvasGroup = watchVisuals.GetComponent<CanvasGroup>();
            if (watchCanvasGroup == null)
            {
                watchCanvasGroup = watchVisuals.AddComponent<CanvasGroup>();
            }
        }

        // Ensure CanvasGroup exists on tracking lost panel
        if (trackingLostPanel != null)
        {
            lostPanelCanvasGroup = trackingLostPanel.GetComponent<CanvasGroup>();
            if (lostPanelCanvasGroup == null)
            {
                lostPanelCanvasGroup = trackingLostPanel.AddComponent<CanvasGroup>();
            }
        }
    }

    /// <summary>
    /// Called when hand tracking is acquired. Connect this to WristTracker.onTrackingAcquired.
    /// </summary>
    public void OnTrackingAcquired()
    {
        if (debounceCoroutine != null)
        {
            StopCoroutine(debounceCoroutine);
        }
        debounceCoroutine = StartCoroutine(DebounceTrackingChange(false));
    }

    /// <summary>
    /// Called when hand tracking is lost. Connect this to WristTracker.onTrackingLost.
    /// </summary>
    public void OnTrackingLost()
    {
        if (debounceCoroutine != null)
        {
            StopCoroutine(debounceCoroutine);
        }
        debounceCoroutine = StartCoroutine(DebounceTrackingChange(true));
    }

    private IEnumerator DebounceTrackingChange(bool trackingLost)
    {
        yield return new WaitForSeconds(debounceTime);

        if (trackingLost != isTrackingLost)
        {
            isTrackingLost = trackingLost;
            ApplyTrackingState();
        }

        debounceCoroutine = null;
    }

    private void ApplyTrackingState()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        float elapsed = 0f;

        float watchStartAlpha = watchCanvasGroup != null ? watchCanvasGroup.alpha : 0f;
        float lostPanelStartAlpha = lostPanelCanvasGroup != null ? lostPanelCanvasGroup.alpha : 0f;

        float watchTargetAlpha = isTrackingLost ? 0f : 1f;
        float lostPanelTargetAlpha = isTrackingLost ? 1f : 0f;

        // Ensure lost panel is active before fading in
        if (isTrackingLost && trackingLostPanel != null)
        {
            trackingLostPanel.SetActive(true);
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Use smooth step for nicer easing
            t = t * t * (3f - 2f * t);

            if (watchCanvasGroup != null)
            {
                watchCanvasGroup.alpha = Mathf.Lerp(watchStartAlpha, watchTargetAlpha, t);
            }

            if (lostPanelCanvasGroup != null)
            {
                lostPanelCanvasGroup.alpha = Mathf.Lerp(lostPanelStartAlpha, lostPanelTargetAlpha, t);
            }

            yield return null;
        }

        // Ensure final values are set
        if (watchCanvasGroup != null)
        {
            watchCanvasGroup.alpha = watchTargetAlpha;
        }

        if (lostPanelCanvasGroup != null)
        {
            lostPanelCanvasGroup.alpha = lostPanelTargetAlpha;
        }

        // Disable lost panel when fully hidden
        if (!isTrackingLost && trackingLostPanel != null)
        {
            trackingLostPanel.SetActive(false);
        }

        fadeCoroutine = null;
    }

    /// <summary>
    /// Immediately show or hide watch without animation.
    /// </summary>
    public void SetWatchVisibleImmediate(bool visible)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (debounceCoroutine != null)
        {
            StopCoroutine(debounceCoroutine);
            debounceCoroutine = null;
        }

        isTrackingLost = !visible;

        if (watchCanvasGroup != null)
        {
            watchCanvasGroup.alpha = visible ? 1f : 0f;
        }

        if (lostPanelCanvasGroup != null)
        {
            lostPanelCanvasGroup.alpha = visible ? 0f : 1f;
        }

        if (trackingLostPanel != null)
        {
            trackingLostPanel.SetActive(!visible);
        }
    }

    /// <summary>
    /// Called when watch becomes visible due to palm facing up.
    /// Connect this to WristTracker.onWatchBecameVisible.
    /// </summary>
    public void OnWatchBecameVisible()
    {
        if (palmDebounceCoroutine != null)
        {
            StopCoroutine(palmDebounceCoroutine);
        }
        palmDebounceCoroutine = StartCoroutine(DebouncePalmChange(true));
    }

    /// <summary>
    /// Called when watch becomes hidden due to palm facing away.
    /// Connect this to WristTracker.onWatchBecameHidden.
    /// </summary>
    public void OnWatchBecameHidden()
    {
        if (palmDebounceCoroutine != null)
        {
            StopCoroutine(palmDebounceCoroutine);
        }
        palmDebounceCoroutine = StartCoroutine(DebouncePalmChange(false));
    }

    private IEnumerator DebouncePalmChange(bool visible)
    {
        yield return new WaitForSeconds(palmGestureDebounceTime);

        bool shouldHide = !visible;
        if (shouldHide != isWatchHiddenByPalm)
        {
            isWatchHiddenByPalm = shouldHide;

            // Only fade watch, don't show tracking lost panel (tracking is still active)
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeWatchOnly(!isWatchHiddenByPalm));
        }

        palmDebounceCoroutine = null;
    }

    private IEnumerator FadeWatchOnly(bool showWatch)
    {
        float elapsed = 0f;
        float startAlpha = watchCanvasGroup != null ? watchCanvasGroup.alpha : 0f;
        float targetAlpha = showWatch ? 1f : 0f;

        while (elapsed < palmGestureFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / palmGestureFadeDuration);
            t = t * t * (3f - 2f * t); // Smooth step

            if (watchCanvasGroup != null)
            {
                watchCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            }

            yield return null;
        }

        if (watchCanvasGroup != null)
        {
            watchCanvasGroup.alpha = targetAlpha;
        }

        fadeCoroutine = null;
    }
}
