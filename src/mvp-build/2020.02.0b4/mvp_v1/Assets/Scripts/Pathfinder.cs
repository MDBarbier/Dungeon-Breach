using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var frontier = new HashSet<Vector3>();
        frontier.Add(transform.position);
        var reached = new HashSet<Vector3>();
        reached.Add(transform.position);

        while (frontier.Count > 0)
        {
            var current = frontier;

            foreach (Vector3 next in GetNeighbours(current))
            {
                if (!reached.Contains(next))
                {
                    frontier.Add(next);
                    reached.Add(next);
                }
            }
        }
    }
    internal HashSet<Vector3> GetNeighbours(HashSet<Vector3> current)
    {
        HashSet<Vector3> result = new HashSet<Vector3>();

        foreach (var square in current)
        {            
            result.Add(new Vector3(square.x + 1, square.y, square.z));
            result.Add(new Vector3(square.x - 1, square.y, square.z));
            result.Add(new Vector3(square.x, square.y, square.z + 1));
            result.Add(new Vector3(square.x, square.y, square.z - 1));
            result.Add(new Vector3(square.x + 1, square.y, square.z + 1));
            result.Add(new Vector3(square.x + 1, square.y, square.z - 1));
            result.Add(new Vector3(square.x - 1, square.y, square.z - 1));
            result.Add(new Vector3(square.x - 1, square.y, square.z + 1));            
        }

        return result;
    }
}
