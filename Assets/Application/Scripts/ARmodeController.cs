using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARmodeController : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager manager;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject HelpPanel;

    void OnEnable() => manager.trackedImagesChanged += OnTrackedImagesChanged;
    void OnDisable() => manager.trackedImagesChanged -= OnTrackedImagesChanged;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            var AR = Instantiate(canvas, trackedImage.transform.position, trackedImage.transform.rotation, trackedImage.transform);
            string name = string.IsNullOrEmpty(trackedImage.referenceImage.name) ? "Null" : trackedImage.referenceImage.name;
            
            AR.SendMessage("LoadData", name);
        }
    }

    private void Update()
    {
        bool anyVisible = false;
        foreach (var tracked in manager.trackables){
            if (tracked.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                anyVisible = true;
                break;
            }
        }

        CanvasGroup group = HelpPanel.GetComponent<CanvasGroup>();
        if ((group.alpha == 1 && anyVisible) || (group.alpha == 0 && !anyVisible))
            StartCoroutine(Fade(!anyVisible));
    }

    IEnumerator Fade(bool visible)
    {
        CanvasGroup group = HelpPanel.GetComponent<CanvasGroup>();
        float start = group.alpha;
        float end = visible ? 1f : 0f;
        float time = 0f;

        while (time < 0.2f)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, time / 0.2f);
            yield return null;
        }

        group.alpha = end;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
}
