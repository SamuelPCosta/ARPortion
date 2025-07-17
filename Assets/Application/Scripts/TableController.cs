using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateCanvasOverlay(){
        var canvas = GameObject.Find("CanvasOverlay");
        if (canvas != null) canvas.transform.GetChild(0).gameObject.SetActive(true);
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
