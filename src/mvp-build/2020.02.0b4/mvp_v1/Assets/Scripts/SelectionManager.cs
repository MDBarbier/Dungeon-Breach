using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private ControlManager controlManager;
    private GameObject lastSelected;
    private GameObject selectionRing;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] GameObject selectionIndicator;    
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        controlManager = FindObjectOfType<ControlManager>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (controlManager.clickDetectedOn == null)
        { 
            return;
        }

        switch (controlManager.clickDetectedOn.tag)
        {
            case "Character":     
                
                if (lastSelected == controlManager.clickDetectedOn)
                {
                    break;
                }

                RemoveSelections();

                selectionRing = Instantiate(selectionIndicator, new Vector3(controlManager.clickDetectedOn.transform.position.x, 0.35f, 
                    controlManager.clickDetectedOn.transform.position.z), Quaternion.identity);

                lastSelected = controlManager.clickDetectedOn;
                break;

            case "Floor":

                RemoveSelections();
                break;

            case "Scenery":
                
                RemoveSelections();
                break;

            case null:
            default:
                RemoveSelections();
                break;
        }
    }

    private void RemoveSelections()
    {
        if (selectionRing != null)
        {
            Destroy(selectionRing);
        }

        lastSelected = null;
    }
}
