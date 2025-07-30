using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARmodeController : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager manager;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject Instructions1;
    [SerializeField] GameObject Instructions2;

    void OnEnable() => manager.trackedImagesChanged += OnTrackedImagesChanged;
    void OnDisable() => manager.trackedImagesChanged -= OnTrackedImagesChanged;

    void Start()
    {
        Instructions2.GetComponent<CanvasGroup>().alpha = 0;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            var AR = Instantiate(canvas, trackedImage.transform.position, trackedImage.transform.rotation, trackedImage.transform);
            string name = string.IsNullOrEmpty(trackedImage.referenceImage.name) ? "Null" : trackedImage.referenceImage.name;
            
            AR.SendMessage("SetData", name);
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

        CanvasGroup group = Instructions1.GetComponent<CanvasGroup>();
        if ((group.alpha == 1 && anyVisible) || (group.alpha == 0 && !anyVisible))
            StartCoroutine(Fade(!anyVisible));
    }

    IEnumerator Fade(bool visible)
    {
        CanvasGroup instructions1 = Instructions1.GetComponent<CanvasGroup>();
        CanvasGroup instructions2 = Instructions2.GetComponent<CanvasGroup>();
        float start1 = instructions1.alpha;
        float end1 = visible ? 1f : 0f;
        float start2 = instructions2.alpha;
        float end2 = visible ? 0f : 1f;
        float time = 0f;

        while (time < 0.2f)
        {
            time += Time.deltaTime;
            float t = time / 0.2f;
            instructions1.alpha = Mathf.Lerp(start1, end1, t);
            instructions2.alpha = Mathf.Lerp(start2, end2, t);
            yield return null;
        }

        instructions1.alpha = end1;
        instructions2.alpha = end2;
        instructions1.interactable = visible;
        instructions1.blocksRaycasts = visible;
        instructions2.interactable = !visible;
        instructions2.blocksRaycasts = !visible;
    }
}
