using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class DetectedController : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager manager;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject canvas;

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
}
