using UnityEngine;
using System.Collections;

public class Pathfinder : MonoBehaviour 
{
    public UnityEngine.AI.NavMeshAgent NavAgent;
    public PathCorner[] WayPoints;

    private bool DestinationSet;
    private bool FromStart = true;
    private UnityEngine.AI.NavMeshPath fromGoal;
    private UnityEngine.AI.NavMeshPath toGoal;
    private UnityEngine.AI.NavMeshPath chosenPath;
    public Transform Target;
    public bool go;
    void Start()
    {
        DestinationSet = false;
        NavAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        fromGoal = new UnityEngine.AI.NavMeshPath();
        toGoal = new UnityEngine.AI.NavMeshPath();
        chosenPath = new UnityEngine.AI.NavMeshPath();
        //NavAgent.enabled = false;
        //int areaMask = NavAgent.areaMask;
        //SetDestination(transform.position, Target.position);
        

    }

	void Update () 
    {
       
        if (DestinationSet)
        {

            Debug.Log("started ");
            for (int i = 0; i < WayPoints.Length; i++)
            {
                UnityEngine.AI.OffMeshLinkData data = NavAgent.nextOffMeshLinkData;
                data = NavAgent.nextOffMeshLinkData;
                NavAgent.Move(WayPoints[i].Position - transform.position);

                if (WayPoints[i].Position == data.startPos)
                {
                    Debug.Log(" JUMP WAY " + WayPoints[i].Position);
                    WayPoints[i].Jump = true;
                }
                if (WayPoints.Length - 1 == i)
                {

                    if (!FromStart)
                    {
                        //Debug.Log("Pos " + WayPoints.Length);
                        PathCorner[] tempWayPoints = new PathCorner[WayPoints.Length];
                        for (int j = WayPoints.Length - 1; j >= 0; j--)
                        {
                            tempWayPoints[WayPoints.Length - j - 1] = WayPoints[j];
                            if (j != 0)
                                tempWayPoints[WayPoints.Length - j - 1].Jump = WayPoints[j - 1].Jump;
                        }
                        WayPoints = tempWayPoints;
                    }
                }
            }
            NavAgent.enabled = false;
            DestinationSet = false;
        }
        for (int i = 0; i < chosenPath.corners.Length - 1; i++)
            Debug.DrawLine(chosenPath.corners[i], chosenPath.corners[i + 1], Color.red);

        if (go) // ORDNING
        {
            NavAgent.enabled = false;
            int areaMask = NavAgent.areaMask;
            SetDestination(transform.position, Target.position);
            go = false;
        }
	}

    public struct PathCorner
    {
        public Vector3 Position;
        public bool Jump;
    }

    public void SetDestination(Vector3 start, Vector3 goal)
    {
        DestinationSet = true;
        fromGoal.ClearCorners();
        toGoal.ClearCorners();
        Debug.Log(transform.position);
        UnityEngine.AI.NavMesh.CalculatePath(start, goal, UnityEngine.AI.NavMesh.AllAreas, toGoal);
        UnityEngine.AI.NavMesh.CalculatePath(goal, start, UnityEngine.AI.NavMesh.AllAreas, fromGoal);

       if(PathLength(fromGoal) > PathLength(toGoal))
       {
           chosenPath = toGoal;
           FromStart = true;
       } 
       else
       {
           chosenPath = fromGoal;
           FromStart = false;
       }
        
        transform.position = chosenPath.corners[0];
        NavAgent.enabled = true;
        NavAgent.SetDestination(chosenPath.corners[chosenPath.corners.Length - 1]);
        WayPoints = new PathCorner[chosenPath.corners.Length];
        for (int i = 0; i < chosenPath.corners.Length; i++)
        {
            
            UnityEngine.AI.OffMeshLinkData data = NavAgent.nextOffMeshLinkData;
            WayPoints[i] = new PathCorner { Position = chosenPath.corners[i],
                                            Jump = false
                                            };
        }
       
        //NavAgent.areaMask = 0;
    }
    private float PathLength(UnityEngine.AI.NavMeshPath path)
    {
        float l = 0;
        for (int i = 0; i < path.corners.Length -1; i++)
        {
            l += (path.corners[i] - path.corners[i + 1]).magnitude;
        }
        return l;
    }
}