using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    private Dictionary<(int, int), GameObject> squares;

    // Start is called before the first frame update
    void Start()
    {
        squares = new Dictionary<(int, int), GameObject>();        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Render a grid of the specified height and width and create one of the specified template game objects for each position
    /// </summary>
    /// <param name="zlength">the height of the grid</param>
    /// <param name="xlength">the width of the grid</param>
    internal Dictionary<(int, int), GameObject> RenderGrid(int zlength, int xlength, GameObject templateGameObject)
    {
        for (int x = 0; x < xlength; x++)
        {
            for (int z = 0; z < zlength; z++)
            {
                var tempGameObject = Instantiate(templateGameObject, new Vector3(x, 0.2f, z), Quaternion.identity);
                var parent = GameObject.Find("InstantiatedFloor");
                tempGameObject.transform.parent = parent.transform;
                tempGameObject.name = $"Floor ({x},{z})";
                squares.Add((x, z), tempGameObject);
            }
        }

        return squares;
    }

    internal void ApplyMaterials(Material material1, Material material2, int xlength, int zlength, Dictionary<(int, int), GameObject> gridPositions)
    {
        bool polarity = true;

        for (int x = 0; x < xlength; x++)
        {
            for (int z = 0; z < zlength; z++)
            {
                GameObject gameObject = gridPositions.Where(a => a.Key.Item1 == x && a.Key.Item2 == z).Select(a => a.Value).FirstOrDefault();

                if (polarity)
                {
                    if (z % 2 == 0)
                    {
                        gameObject.GetComponent<MeshRenderer>().material = material1;
                    }
                    else
                    {
                        gameObject.GetComponent<MeshRenderer>().material = material2;
                    }
                }
                else
                {
                    if (z % 2 == 0)
                    {
                        gameObject.GetComponent<MeshRenderer>().material = material2;
                    }
                    else
                    {
                        gameObject.GetComponent<MeshRenderer>().material = material1;
                    }
                }
            }

            polarity = !polarity;
        }
    }
}
