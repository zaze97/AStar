using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AstarAI : MonoBehaviour
{
    public Transform targetPosition;

    private Seeker seeker;
    private CharacterController controller;

    public Path path;

    public float speed = 2;

    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public bool reachedEndOfPath;
    public void Start () {
        // 获取我们之前添加的Seeker组件的引用
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        // 开始计算targetPosition对象的新路径，将结果返回给OnPathComplete方法。
        // 路径请求是异步的，所以OnPathComplete方法何时被调用取决于它有多长
        // 取计算路径。通常它被称为下一个坐标系。
        seeker.pathCallback += OnPathComplete;
        seeker.StartPath(transform.position, targetPosition.position);
        
        //两种写法，先注册回调或path时注册回调
        //seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        
        
    }
    public void OnPathComplete (Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            // 重置路径点计数器，以便我们开始移动到路径上的第一个点
            currentWaypoint = 0;
        }
    }

    public void Update () {
        if (path == null) {
            //我们还没有路可走，所以什么都不要做
            return;
        } 
        //在一个循环中检查，如果我们足够接近当前的路点切换到下一个。
        //我们在一个循环中这样做，因为许多路点可能彼此很近，我们可能到达
        //在同一帧中有几个。
        reachedEndOfPath = false;
        //路径上到下一个路径点的距离
        float distanceToWaypoint;
        while (true) {
        //如果你想要最大的性能，你可以检查平方距离，而不是去掉a
        //平方根计算。但这超出了本教程的范围。
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
        //检查是否有其他路点或我们是否已经到达路径的终点
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
        //设置一个状态变量来指示代理已经到达路径的终点。
        //你可以使用这个来触发一些特殊的代码，如果你的游戏需要。
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }

        //在接近路径的尽头时，平稳地放慢速度
        //当代理接近路径上的最后一个路径点时，这个值将平滑地从1到0。
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;

        //指向下一个路径点
        //将其规范化，使其长度为1世界单位
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        //方向乘以我们的期望速度，得到一个速度
        Vector3 velocity = dir * speed * speedFactor;
        //使用CharacterController组件移动代理
        //注意SimpleMove的速度是以米/秒为单位，所以我们不应该乘以Time.deltaTime

        // Move the agent using the CharacterController component
        // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
        controller.SimpleMove(velocity);

        //如果你正在编写一款2D游戏，你应该删除上面的CharacterController代码，并通过取消注释直接移动转换
        // transform.position += velocity * Time.deltaTime;
    }
    public void OnDisable () {
        seeker.pathCallback -= OnPathComplete;
    }
}
