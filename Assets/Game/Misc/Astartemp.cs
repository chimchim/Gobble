using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AStar : MonoBehaviour
{
   /* public Dictionary<Vector2, BlockData> nodeDic = new Dictionary<Vector2, BlockData>();
    public GameObject plane;
    private string hover = "default";
    private bool StartSearch = false;
    bool finished = false;
    private Queue<BlockData> colorNodes = new Queue<BlockData>();
    private float timer = 0;
    private Vector2[] neighbours = {
new Vector2(-1, 1),
new Vector2(0, 1),
new Vector2(1, 1),
new Vector2(-1, 0),
new Vector2(1, 0),
new Vector2(-1, -1),
new Vector2(0, -1),
new Vector2(1, -1)
};
    // Use this for initialization
    void Start()
    {
        Vector3 pos = plane.transform.position;
        Vector3 start = plane.transform.position;
        for (int y = 0; y < 20; y++)
        {
            pos = new Vector3(start.x, (pos.y - 1.1f), pos.z);
            for (int x = 0; x < 20; x++)
            {
                pos = new Vector3((pos.x + 1.1f), pos.y, pos.z);
                GameObject go = Instantiate(plane, pos, plane.transform.rotation) as GameObject;
                go.GetComponent<BlockData>().position = new Vector2(x, y);
                go.GetComponent<BlockData>().setBlock("normal");
                if (x == 0 && y == 0)
                {
                    go.GetComponent<BlockData>().setBlock("start");
                    go.name = "start";
                }
                if (x == 0 && y == 1)
                {
                    go.GetComponent<BlockData>().setBlock("end");
                    go.name = "end";
                }
                nodeDic.Add(new Vector2(x, y), go.GetComponent<BlockData>());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Om vi inte har startat sökningen, låt användaren redigera rutnätet.
        if (!StartSearch)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.gameObject.GetComponent<BlockData>().end && !hit.collider.gameObject.GetComponent<BlockData>().start && hover == "default" && !hit.collider.gameObject.GetComponent<BlockData>().wall)
                    {
                        hit.collider.gameObject.GetComponent<BlockData>().setBlock("wall");
                    }
                    else if (hit.collider.gameObject.GetComponent<BlockData>().end || hit.collider.gameObject.GetComponent<BlockData>().start)
                    {
                        hit.collider.gameObject.GetComponent<BlockData>().setBlock("normal");
                        hover = hit.collider.gameObject.name;
                        hit.collider.gameObject.name = "Plane(Clone)";
                    }
                    else if (hover != "default")
                    {
                        hit.collider.gameObject.GetComponent<BlockData>().setBlock(hover);
                        hit.collider.gameObject.name = hover;
                        hover = "default";
                    }
                    else if (hover == "default" && hit.collider.gameObject.GetComponent<BlockData>().wall)
                    {
                        hit.collider.gameObject.GetComponent<BlockData>().setBlock("normal");
                    }
                }
            }
            if (hover != "default")
            {
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit1;
                if (Physics.Raycast(ray1, out hit1))
                {
                    hit1.collider.gameObject.GetComponent<BlockData>().Hover(hover);
                }
            }
            // starta sökningen
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartSearch = true;
                findPath();
            }
        }
    }
    private void findPath()
    {
        PriorityQueue<BlockData> openList = new PriorityQueue<BlockData>();
        List<BlockData> closedList = new List<BlockData>();
        BlockData endNode = new BlockData();
        BlockData startNode = new BlockData();
        // Ta reda på vilka block som är start samt slut.
        foreach (BlockData block in nodeDic.Values)
        {
            if (block.start)
            {
                block.walked = 0;
                startNode = block;
                openList.Enqueue(block, 0f);
            }
            if (block.end)
            {
                endNode = block;
            }
        }
        //Leta tills vi är klara
        while (!finished)
        {
            // hämta ut det block med minst kostnad, i openlist.
            BlockData bd = openList.Dequeue();
            //För varje granne
            for (int i = 0; i < 8; i++)
            {
                //grannens position
                Vector2 pos = neighbours[i] + bd.position;
                // existerar grannen? (om vi är vid en kant)
                if (nodeDic.ContainsKey(pos))
                {
                    BlockData child = nodeDic[pos];
                    // är grannen inte en vägg?
                    if (!child.wall)
                    {
                        // är den slutmålet?
                        if (child.end)
                        {
                            child.parent = bd;
                            child.walked = child.parent.walked + 1;
                            finished = true;
                            break;
                        }
                        // grannen distans från slutmålet
                        float dst = Vector2.Distance(child.position, endNode.position);
                        // har vi besökt noden tidigare? om inte,färglägg, parenta, updatera nodens F värde (walked) och lägg till i vår open list
                        if (!openList.Contains(child) && !closedList.Contains(child))
                        {
                            child.setBlock("open");
                            child.parent = bd;
                            child.walked = child.parent.walked + Vector2.Distance(bd.position, child.position) * 10;
                            openList.Enqueue(child, dst + child.walked);
                        }
                    }
                }
            }
            if (!finished)
            {
                // lägg till söknoden i closedlist
                bd.setBlock("closed");
                closedList.Add(bd);
            }
        }
        // när vi är klara, backtraca och rita ut.
        BlockData path = endNode;
        for (int i = 0; i < endNode.walked; i++)
        {
            path.parent.setBlock("path");
            path = path.parent;
        }
    }*/
}