using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed = 10;
    public float turnDis = 5;
    public float turnSpeed = 5;

    Path path;



    private void Start()
    {
        PathManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public  void OnPathFound(Vector3[] wayPoints,bool pathSuccessful)
    {
        
        if (pathSuccessful)
        {

            path = new Path(wayPoints, transform.position, turnDis);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {

        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);
        while (followingPath)
        {
            Vector2 pos2d = new Vector2(transform.position.x, transform.position.z);
            if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2d))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            }
           
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {

            path.DrawWithGizmos();
        }
    }
}
