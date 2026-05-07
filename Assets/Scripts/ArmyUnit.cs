using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArmyUnit : MonoBehaviour
{
    public enum AttackType { Melee, Ranged }

    public bool playerControlled;

    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    public float attackDamage = 10f;
    public float attackSpeed = 1f; // attacks per second
    public AttackType attackType;

    [Header("Combat")]
    public float attackRange = 1.5f;

    private ArmyUnit currentTarget;
    private float attackCooldown;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.1f;

    [Header("Separation (Anti-Clumping)")]
    public float separationRadius = 0.3f;
    public float separationStrength = 2f;


    [Header("Wander Area")]
    public float wanderRadius = 3f;

    [Header("Timing")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private Vector2 spawnPosition;
    private Vector2 targetPosition;
    private bool isWaiting = false;
    private float waitTime;

    private Collider2D[] nearbyUnits = new Collider2D[32];

    private float retargetTimer;
    
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
        retargetTimer = Random.Range(0f, 0.15f);
    }

    private void Update()
    { 
        if (GameManager.Instance.inCombat)
        {
            HandleCombat();
        }
        else
        {
            HandleWander();
        }
    }

    public void Fill(UnitData data)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        attackSpeed = data.attackSpeed;
        moveSpeed = data.moveSpeed;
    }

    Vector2 ComputeSeparation()
    {
        Vector2 force = Vector2.zero;
        int count = 0;

        int hits = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            separationRadius,
            nearbyUnits
        );

        for (int i = 0; i < hits; i++)
        {
            ArmyUnit unit = nearbyUnits[i].GetComponent<ArmyUnit>();

            if (unit == null || unit == this)
                continue;
            if (unit.playerControlled != this.playerControlled) continue;


            Vector2 offset = (Vector2)transform.position - (Vector2)unit.transform.position;
            float sqrDist = offset.sqrMagnitude;

            if (sqrDist > 0.0001f)
            {
                force += offset.normalized / Mathf.Sqrt(sqrDist);
                count++;
            }
        }

        if (count > 0)
        {
            force /= count;
        }

        return force * separationStrength;
    }

    void HandleCombat()
    {
        attackCooldown -= Time.deltaTime;
        retargetTimer -= Time.deltaTime;

        if (retargetTimer <= 0f)
        {
            currentTarget = FindClosestEnemy();
            retargetTimer = 0.15f;
        }

        if (currentTarget == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange)
        {
            AttackTarget();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    ArmyUnit FindClosestEnemy()
    {
        ArmyUnit closest = null;
        float closestSqrDist = Mathf.Infinity;

        Vector2 myPos = transform.position;


        foreach (var unit in GameManager.Instance.allUnits)
        {
            if (unit == this) continue;
            if (unit.playerControlled == this.playerControlled) continue;
            Vector2 offset = (Vector2)unit.transform.position - myPos;

            float sqrDist = offset.sqrMagnitude;

            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest = unit;
            }
        }

        return closest;
    }

    void MoveTowardsTarget()
    {
        Vector2 toTarget = (currentTarget.transform.position - transform.position).normalized;

        Vector2 separation = ComputeSeparation();

        Vector2 finalDirection = (toTarget + separation).normalized;

        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
    }

    void AttackTarget()
    {

        if (attackCooldown <= 0f)
        {
            attackCooldown = 1f / attackSpeed;

            if (attackType == AttackType.Melee)
            {
                currentTarget.TakeDamage(attackDamage);
            }
            else if (attackType == AttackType.Ranged)
            {
                //Swap to projectile animation later
                currentTarget.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
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
        Vector2 toTarget = (targetPosition - (Vector2)transform.position).normalized;

        Vector2 separation = ComputeSeparation();

        Vector2 finalDirection = (toTarget + separation).normalized;

        transform.position += (Vector3)(finalDirection * moveSpeed * Time.deltaTime);

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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPosition : transform.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
