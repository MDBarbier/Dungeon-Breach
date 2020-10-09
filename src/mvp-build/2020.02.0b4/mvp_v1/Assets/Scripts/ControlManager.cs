using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private Camera mainCamera;
    internal GameObject clickDetectedOn;
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

        if (Input.GetMouseButtonDown(0))
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceBarDetected = true;
        }
    }

    internal void ResetSpacebarDetection()
    {
        SpaceBarDetected = false;
    }

    internal bool GetSpacebarDetected() => SpaceBarDetected;
}
