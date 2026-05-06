using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyUnit : MonoBehaviour
{
    public enum AttackType { Melee, Ranged }

    public bool playerControlled;

    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    public float attackDamage = 10f;
    public float attackSpeed = 1f; // attacks per second
    public AttackType attackType;

    public float attackRange = 1.5f;

    [Header("Combat")]
    public float detectionRange = 10f;

    private ArmyUnit currentTarget;
    private float attackCooldown;

    private bool inCombat = false;


    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.1f;

    [Header("Wander Area")]
    public float wanderRadius = 3f;

    [Header("Timing")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private Vector2 spawnPosition;
    private Vector2 targetPosition;
    private bool isWaiting = false;
    private float waitTime;

    void OnEnable()
    {
       GameManager.Instance.allUnits.Add(this);
    }

    void OnDisable()
    {
        GameManager.Instance.allUnits.Remove(this);
    }

    void Start()
    {
        spawnPosition = transform.position;
        PickNewTarget();
        currentHealth = maxHealth;
    }

    private void Update()
    { 
        if (inCombat)
        {

        }
        else
        {
            HandleWander();
        }
    }

    #region Wandering code
    
    private void HandleWander()
    {
        if (isWaiting)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) 
            {
                PickNewTarget();
                isWaiting = false;
            }
        }
        else
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance <= stoppingDistance)
        {
            StartWaiting();
        }
    }

    private void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPosition + randomOffset;
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPosition : transform.position, wanderRadius);
    }
}
