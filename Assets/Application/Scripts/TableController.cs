using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    int TABLE_CHILD = 2;

    public void ActivateCanvasOverlay(){
        var canvas = GameObject.Find("CanvasOverlay");
        if (canvas != null) { 
            canvas.transform.GetChild(TABLE_CHILD).gameObject.SetActive(true);
            canvas.GetComponent<FullScreenController>().setTutorial();
        }
        else
            print("objeto nulo");
    }

    public void DeactivateCanvasOverlay()
    {
        var canvas = GameObject.Find("CanvasOverlay");
        if (canvas != null) canvas.transform.GetChild(0).gameObject.SetActive(false);
        else
            print("objeto nulo");
    }

    public void SetPanelPosition(int value)
    {
       GameObject.FindObjectOfType<FullScreenController>().SetPanelPosition(value);

    }
}
