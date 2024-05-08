using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerMovement : MonoBehaviour
{
    Vector2 lastclickedpos;

    public float speed = 1000f;
    public float nextWaypointDistance = 1f;


    Path path;
    int currentWaypoint = 0;
    //bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    IEnumerator init()
    {
        yield return new WaitForSeconds(3f);
        AstarPath.active.Scan();
    }
    
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        StartCoroutine(init());
    }
    void UpdatePath()
    {
        float Distance = Vector3.Distance(lastclickedpos, rb.position);

        if (rb.position != lastclickedpos)
        {
            if (seeker.IsDone())
                seeker.StartPath(rb.position, lastclickedpos, OnPathComplete);
        }

    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastclickedpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (path == null)
            return;

        //if (currentWaypoint >= path.vectorPath.Count)
        //{
        //    reachedEndOfPath = true;
        //    return;
        //}
        //else
        //{
        //    reachedEndOfPath = false;
        //}


        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);


        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
