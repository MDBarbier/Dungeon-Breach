using System;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject clickDetectedOn;
    internal GameObject hoverDetectedOn;
    private bool SpaceBarDetected;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpaceBarDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform)
                {                    
                    clickDetectedOn = hit.transform.gameObject;
                }
            }

            if (clickDetectedOn != null && debugLogging)
            {
                print($"click detected on {clickDetectedOn.name}");
            }
        }

        HandleHover(ray);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceBarDetected = true;
        }
    }

    private void HandleHover(Ray ray)
    {
        RaycastHit hit;        

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform)
            {
                hoverDetectedOn = hit.transform.gameObject;
            }
        }
        else
        {
            hoverDetectedOn = null;
        }
        
        if (hoverDetectedOn != null && debugLogging)
        {
            print($"hover detected on {hoverDetectedOn.name}");
        }
    }

    internal void ResetSpacebarDetection()
    {
        SpaceBarDetected = false;
    }

    internal void ClearClickDetectedOn()
    {
        clickDetectedOn = null;
    }

    internal bool GetSpacebarDetected() => SpaceBarDetected;

    internal GameObject GetClickDetectedOn() => clickDetectedOn;
}
