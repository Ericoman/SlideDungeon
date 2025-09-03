using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolController : MonoBehaviour
{
    public Animator animator;

    //movement
    NavMeshAgent navMeshAgent;
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
            animator.SetBool("animPatrolling", _isPatrol);
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
                    animator.SetBool("animPatrolling", _isPatrol);

                }
                else if (Vector3.Angle(transform.forward, dirToPlayer) < viewangle / 2)
                {
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMasck)) //No obstacles
                    {
                        _isPatrol = false;
                        animator.SetBool("animPatrolling", _isPatrol);
                        _playerPosition = player.transform.position;
                    }
                    else
                    {
                        _isPatrol = true;
                        animator.SetBool("animPatrolling", _isPatrol);
                    }
                }
            }
        }
        
    }


    void Chase() 
    {
        if (_playerPosition != Vector3.zero) 
        {
            Move(speedRun);
            navMeshAgent.SetDestination(_playerPosition);
            _lostPlayer = false;

            if(navMeshAgent.remainingDistance <= damageRadius) 
            {
                Collider[] playerInRange = Physics.OverlapSphere(transform.position, damageRadius, playerMask);
                if (playerInRange.Length != 0)
                {
                    HealthComponent health = playerInRange[0].gameObject.GetComponentInChildren<HealthComponent>();
                    if (health.ReceiveDamage(damage)) 
                    {
                      animator.SetBool("animBite", true);
                        
                      StartCoroutine(EndAnimBite());
                    }
                    Stop();
                }
            }
        }
    }

    void Patrol() 
    {
        _playerPosition = Vector3.zero;
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
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    void NextPoint()
    {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
    }

    private IEnumerator EndAnimBite()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("animBite", false);
        Move(speedRun);
    }
}
