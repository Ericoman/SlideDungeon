using UnityEngine;
using UnityEngine.AI;

public class ShooterController : MonoBehaviour
{
    public Animator animator;

    //movement
    NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float speedWalk = 6;
    float _waitTime;

    //player
    public int damage = 1;

    //waypoints
    public Transform[] waypoints;
    int _currentWaypointIndex;

    // shooting
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootInterval = 2f;
    float _shootTimer;

    void Start()
    {
        _waitTime = startWaitTime;
        _shootTimer = shootInterval;
        _currentWaypointIndex = 0;

        if (waypoints.Length == 0)
        {
            GameObject punto = new GameObject("Waypoint0");
            punto.transform.position = transform.position;

            waypoints = new Transform[1];
            waypoints[0] = punto.transform;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.updateRotation = false;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);

    }

    void Update()
    {
        Patrol();
        HandleShooting();
    }

    void HandleShooting()
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            Shoot();
            _shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            projectileObj.TryGetComponent<Projectile>(out Projectile projectile);
            if (projectile) 
            {
                projectile.SetDamage(damage);
            }
        }
    }

    void Patrol()
    {
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1)
        {
            if (_waitTime <= 0)
            {
                NextPoint();
                Move(speedWalk);
                _waitTime = startWaitTime;
            }
            else
            {
                Stop();
                _waitTime -= Time.deltaTime;
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    void NextPoint()
    {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
    }

}
