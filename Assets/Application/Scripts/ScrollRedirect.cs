using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRedirect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect horizontalScrollRect;
    private ScrollRect myScrollRect;
    private bool redirectToHorizontal = false;

    private void Awake()
    {
        myScrollRect = GetComponent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Verifica se o movimento inicial é mais horizontal que vertical
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            redirectToHorizontal = true;
            myScrollRect.enabled = false;
            horizontalScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            redirectToHorizontal = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (redirectToHorizontal)
        {
            horizontalScrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (redirectToHorizontal)
        {
            myScrollRect.enabled = true;
            horizontalScrollRect.OnEndDrag(eventData);
            redirectToHorizontal = false;
        }
    }
}