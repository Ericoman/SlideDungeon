using UnityEngine;
using UnityEngine.AI;

public class PatrolController : MonoBehaviour
{
    //movement
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewangle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMasck;

    //player
    public int damage = 1;
    public float damageRadius = 2;    
    Vector3 _playerPosition;

    //waypoints
    public Transform[] waypoints;
    int _currentWaypointIndex;

    //private
    float _waitTime;
    bool _isPatrol;
    bool _lostPlayer;


    void Start()
    {
        _isPatrol = true;
        _lostPlayer = true;
        _playerPosition = Vector3.zero;
        _waitTime = startWaitTime;
        _currentWaypointIndex = 0;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);

    }

    void Update()
    {
        CheckPlayer();

        if (_isPatrol)
        {
            Patrol();
        }
        else 
        {
            Chase();
        }
        
    }

    void CheckPlayer() 
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if(playerInRange.Length == 0) 
        {
            _isPatrol = true;
        }
        else 
        { 
            for (int i = 0; i < playerInRange.Length; i++)
            {
                //distance and angle to de player
                Transform player = playerInRange[i].transform;
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float dstToPlayer = Vector3.Distance(transform.position, player.position);

                if (dstToPlayer > viewRadius) //fuera de radio
                {
                    _isPatrol = true;

                }
                else if (Vector3.Angle(transform.forward, dirToPlayer) < viewangle / 2)
                {
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMasck)) //No obstacles
                    {
                        _isPatrol = false;
                        _playerPosition = player.transform.position;
                    }
                    else
                    {
                        _isPatrol = true;
                    }
                }
            }
        }
        
    }


    void Chase() 
    {
        if(_playerPosition != Vector3.zero) 
        {
            Move(speedRun);
            navMeshAgent.SetDestination(_playerPosition);
            _lostPlayer = false;

            if(navMeshAgent.remainingDistance <= damageRadius) 
            {
                Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
                if (playerInRange.Length != 0)
                {
                    HealthComponent health = playerInRange[0].gameObject.GetComponentInChildren<HealthComponent>();
                    health.ReceiveDamage(damage);
                }
            }
        }
    }

    void Patrol() 
    {
        _playerPosition = Vector3.zero;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
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
        else if (!_lostPlayer) //player just lost 
        {
            _lostPlayer = true;
            Move(speedWalk);
            _waitTime = startWaitTime;
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
