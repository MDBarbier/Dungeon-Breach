using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Destroy()
    {
        print("Destroying " + gameObject.name);
        Destroy(gameObject);
    }
}
