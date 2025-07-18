using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.IO;

public class TargetManager : MonoBehaviour
{
    // Prefabs to spawn
    [SerializeField] GameObject canvas; //Serializable

    // ARTrackedImageManager reference
    private ARTrackedImageManager _trackedImageManager;

    // Dictionary to reference spawned prefabs with tracked image name
    private Dictionary<string, GameObject> _arObjects;

    // Initialization and references assigning
    // Event function
    private void Start()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (_trackedImageManager == null) return;
        _trackedImageManager.trackedImagesChanged += OnImagesTrackedChanged;
        _arObjects = new Dictionary<string, GameObject>();

        SetupSceneElements();
    }

    private void OnDestroy()
    {
        _trackedImageManager.trackedImagesChanged -= OnImagesTrackedChanged;
    }

    // Setup Scene Elements
    private void SetupSceneElements()
    {
        //foreach (var prefab in prefabsToSpawn)
        //{
            GameObject arObject = Instantiate(canvas, Vector3.zero, Quaternion.identity);
            arObject.name = "CanvasProduct";
            arObject.gameObject.SetActive(false);
            _arObjects.Add(arObject.name, arObject);
        //}
    }

    private void OnImagesTrackedChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateTrackedImages(trackedImage);
        }
    }

    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

        if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }

        _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
        _arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        _arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
    }
    
}