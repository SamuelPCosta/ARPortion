using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DetectedController : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager manager;

    void OnEnable() => manager.trackedImagesChanged += OnTrackedImagesChanged;
    void OnDisable() => manager.trackedImagesChanged -= OnTrackedImagesChanged;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
            Debug.Log("Imagem detectada: " + trackedImage.referenceImage.name);
        foreach (var trackedImage in args.added)
        {
            Debug.Log($"Imagem detectada: guid={trackedImage.referenceImage.guid} | name='{trackedImage.referenceImage.name}'");
        }
    }
}
