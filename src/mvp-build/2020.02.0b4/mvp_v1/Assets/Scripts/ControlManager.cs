using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private Camera mainCamera;
    internal GameObject clickDetectedOn;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] bool debugLogging = false;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
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

                    //Use this to detect hits on nested game objects which are part of a nested prefab for example
                    //todo :refactor: make recursive
                    if (hit.transform.tag == "NestedGameObject")
                    {
                        clickDetectedOn = hit.transform.parent.gameObject;                        
                    }
                }
            }

            if (clickDetectedOn != null && debugLogging)
            {
                print($"click detected on {clickDetectedOn.name}");
            }
        }
    }
}
