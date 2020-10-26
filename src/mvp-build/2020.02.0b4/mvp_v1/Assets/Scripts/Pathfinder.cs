using Assets.Scripts.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

    private DungeonManager dungeonManager;
    private CharacterManager characterManager;
    private Dictionary<(int, int), GameObject> grid;
    //private GameObject[][] obstacles;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] Material highlightedSquare;    
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        dungeonManager = FindObjectOfType<DungeonManager>();
        grid = dungeonManager.GetDungeonGrid();   
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    internal ValueTuple<GameObject, Character> FindNearestEnemy(Vector3 startingPosition)
    {
        //frontier represents the squares at the edge of our exploration
        var frontier = new Queue<Vector3>();

        //the camefrom dictionary tracks where each and every square was moved to from
        Dictionary<Vector3, Vector3?> cameFrom = new Dictionary<Vector3, Vector3?>();

        //set the starting square to the current position of the transform
        var startingSquare = startingPosition;

        //add our starting square, with a null value
        cameFrom.Add(startingSquare, null);

        //add our starting square to the frontier
        frontier.Enqueue(startingSquare);

        //control loop that will keep executing as long as we don't run out of frontier (or we find our goal)
        while (frontier.Count > 0)
        {
            //set the current square to the next in the frontier queue (this also removes it from the frontier queue)
            var currentSq = frontier.Dequeue();            

            //early exit clause if the current square is our goal
            var charAtPosition = characterManager.GetCharacterAtPosition((int)currentSq.x, (int)currentSq.z);

            if (charAtPosition.Item1 != null && charAtPosition.Item2.PlayerControlled)
            {
                return charAtPosition;
            }

            //loop through the current square's neighbours...
            foreach (Vector3 next in GetNeighbours(currentSq, false))
            {
                //and if they aren't already in the cameFrom dictionary...
                if (!cameFrom.ContainsKey(next))
                {
                    //add them to our frontier to be explored
                    frontier.Enqueue(next);

                    //and add them to the cameFrom to record our progress, linking our current square as the predecessor to this next square
                    cameFrom.Add(next, currentSq);
                }
            }
        }

        return (null, null);
    }

    internal List<Vector3> Pathfind(Vector3 goal, Vector3 startingPosition, bool includeDestination, bool includeOrigin)
    {
        bool pathComplete = false;

        //frontier represents the squares at the edge of our exploration
        var frontier = new Queue<Vector3>();

        //the camefrom dictionary tracks where each and every square was moved to from
        Dictionary<Vector3, Vector3?> cameFrom = new Dictionary<Vector3, Vector3?>();

        //set the starting square to the current position of the transform
        var startingSquare = startingPosition;

        //add our starting square, with a null value
        cameFrom.Add(startingSquare, null);

        //add our starting square to the frontier
        frontier.Enqueue(startingSquare);        

        //control loop that will keep executing as long as we don't run out of frontier (or we find our goal)
        while (frontier.Count > 0)
        {
            //set the current square to the next in the frontier queue (this also removes it from the frontier queue)
            var currentSq = frontier.Dequeue();                        
            
            //early exit clause if the current square is our goal
            if (currentSq.x == goal.x && currentSq.z == goal.z)
            {
                pathComplete = true;
                break;
            }

            //loop through the current square's neighbours...
            var neighbours = GetNeighbours(currentSq, false);
            foreach (Vector3 next in neighbours)
            {
                //if the neighbouring square is not the goal and there is a character in it, ignore it
                if (goal.x != next.x || goal.z != next.z)
                {
                    var charInSquare = characterManager.GetCharacterAtPosition((int)next.x, (int)next.z);

                    if (charInSquare.Item1 != null)
                    {
                        continue;
                    }
                }                

                //and if they aren't already in the cameFrom dictionary...
                if (!cameFrom.ContainsKey(next))
                {
                    //add them to our frontier to be explored
                    frontier.Enqueue(next);

                    //and add them to the cameFrom to record our progress, linking our current square as the predecessor to this next square
                    cameFrom.Add(next, currentSq);
                }
            }
        }

        if (!pathComplete)
        {
            throw new Exception("failed to find goal");
        }

        //now we have our path, we set the current square to be the goal
        var current = goal;

        //set up a list to hold our path (List is important as it preserves insertion order)
        List<Vector3> path = new List<Vector3>();

        //loop through until we get back to starting point
        int safetyCounter = 0;
        while (current.x != startingSquare.x || current.z != startingSquare.z && safetyCounter < 1000)
        {
            safetyCounter++;

            //add the current node to the path
            if (current.x == goal.x && current.z == goal.z)
            {
                if (includeDestination)
                {
                    path.Add(current);  
                }
            }
            else
            {
                path.Add(current);
            }

            //set the new current node to the value of the corresponding dictionary entry
            cameFrom.TryGetValue(new Vector3(current.x, 0f, current.z), out var temp);

            if (temp != null)
            {
                current = (Vector3)temp;
            }
        }

        //lastly add our starting square (optional)
        if (includeOrigin)
        {
            path.Add(startingSquare); 
        }

        //reverse the list so we have the path in the correct order
        path.Reverse();

        return path;
    }

    internal Dictionary<Vector3, (Vector3?, int)> GetPlayerMoveArea(Vector3 startingPosition)
    {
        //frontier represents the squares at the edge of our exploration
        var frontier = new Queue<Vector3>();

        //the camefrom dictionary tracks where each and every square was moved to from
        Dictionary<Vector3, (Vector3?, int)> cameFrom = new Dictionary<Vector3, (Vector3?, int)>();

        //set the starting square to the current position of the transform
        var startingSquare = startingPosition;

        //add our starting square, with a null value
        cameFrom.Add(startingSquare, (null, 0));

        //add our starting square to the frontier
        frontier.Enqueue(startingSquare);

        //control loop that will keep executing as long as we don't run out of frontier (or we find our goal)
        while (frontier.Count > 0)
        {
            //set the current square to the next in the frontier queue (this also removes it from the frontier queue)
            var currentSq = frontier.Dequeue();

            //loop through the current square's neighbours...
            var neighbours = GetNeighbours(currentSq, false);
            foreach (Vector3 next in neighbours)
            {
                //and if they aren't already in the cameFrom dictionary...
                if (!cameFrom.ContainsKey(next))
                {
                    //add them to our frontier to be explored
                    frontier.Enqueue(next);

                    var reference = cameFrom.Where(a => a.Key == currentSq).FirstOrDefault();

                    //and add them to the cameFrom to record our progress, linking our current square as the predecessor to this next square
                    cameFrom.Add(next, (currentSq, reference.Value.Item2 + 1));
                }
            }
        }

        return cameFrom;
    }

    private IEnumerable<Vector3> GetNeighbours(Vector3 origin, bool eightWay)
    {
        List<Vector3> list = new List<Vector3>();
        List<Vector3> finalList = new List<Vector3>();

        list.Add(new Vector3(origin.x + 1f, 0f, origin.z));
        list.Add(new Vector3(origin.x - 1f, 0f, origin.z));
        list.Add(new Vector3(origin.x, 0f, origin.z + 1f));
        list.Add(new Vector3(origin.x, 0f, origin.z - 1f));

        if (eightWay)
        {

            list.Add(new Vector3(origin.x + 1f, 0f, origin.z + 1f));
            list.Add(new Vector3(origin.x + 1f, 0f, origin.z - 1f));
            list.Add(new Vector3(origin.x - 1f, 0f, origin.z + 1f));
            list.Add(new Vector3(origin.x - 1f, 0f, origin.z - 1f));

        }

        foreach (var possibility in list)
        {
            if (dungeonManager.GetFloorTileByLocation(possibility.x, possibility.z) != null)
            {
                if (!dungeonManager.IsObstacleInSpace(possibility))
                {
                    finalList.Add(possibility); 
                }
            }
        }

        return finalList;
    }    
}
