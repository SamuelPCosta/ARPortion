using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    public class PlaneVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer m_LineRendererPrefab;
        Dictionary<ARPlane, LineRenderer> m_PlaneLineRenderers = new();
        // Start is called before the first frame update
        void Start()
        {

        }

        XROrigin m_Origin;
        GameObject m_PlaneVisualizers;
        private void configure()
        {
            var planeManager = m_Origin.GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                if (m_PlaneVisualizers == null)
                {
                    m_PlaneVisualizers = new GameObject("PlaneVisualizers");
                }

                m_PlaneVisualizers.SetActive(false);
                planeManager.planesChanged += OnPlaneChanged;
            }
        }

        void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
        {
            foreach (var plane in eventArgs.added)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }

            foreach (var plane in eventArgs.updated)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }

            foreach (var plane in eventArgs.removed)
            {
                if (m_PlaneLineRenderers.TryGetValue(plane, out var lineRenderer))
                {
                    m_PlaneLineRenderers.Remove(plane);
                    if (lineRenderer)
                    {
                        Destroy(lineRenderer.gameObject);
                    }
                }
            }
        }

        LineRenderer GetOrCreateLineRenderer(ARPlane plane)
        {
            if (m_PlaneLineRenderers.TryGetValue(plane, out var foundLineRenderer) && foundLineRenderer)
            {
                return foundLineRenderer;
            }

            var go = Instantiate(m_LineRendererPrefab, m_PlaneVisualizers.transform);
            var lineRenderer = go.GetComponent<LineRenderer>();
            m_PlaneLineRenderers[plane] = lineRenderer;

            return lineRenderer;
        }

        void UpdateLine(ARPlane plane, LineRenderer lineRenderer)
        {
            if (!lineRenderer)
            {
                return;
            }

            Transform planeTransform = plane.transform;
            bool useWorldSpace = lineRenderer.useWorldSpace;
            if (!useWorldSpace)
            {
                lineRenderer.transform.SetPositionAndRotation(planeTransform.position, planeTransform.rotation);
            }

            var boundary = plane.boundary;
            lineRenderer.positionCount = boundary.Length;
            for (int i = 0; i < boundary.Length; ++i)
            {
                var point2 = boundary[i];
                var localPoint = new Vector3(point2.x, 0, point2.y);
                if (useWorldSpace)
                {
                    lineRenderer.SetPosition(i, planeTransform.position + (planeTransform.rotation * localPoint));
                }
                else
                {
                    lineRenderer.SetPosition(i, new Vector3(point2.x, 0, point2.y));
                }
            }
        }
    }
}