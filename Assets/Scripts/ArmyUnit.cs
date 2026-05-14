using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class ArmyUnit : MonoBehaviour
{
    public enum AttackType { Melee, Ranged }

    public bool playerControlled;
    public bool pause = false;

    [Header("Basic")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color enemyColor;

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


    private Lane lane;

    private Vector2 spawnPosition;
    private Vector2 targetPosition;
    private bool isWaiting = false;
    private float waitTime;

    private Collider2D[] nearbyUnits = new Collider2D[32];

    private Transform cachedTransform;

    private float retargetTimer;
    float attackRangeSqr;

    void Start()
    {
        cachedTransform = transform;
        spawnPosition = transform.position;
        PickNewTarget();
        currentHealth = maxHealth;
        retargetTimer = Random.Range(0f, 0.15f);
        attackRangeSqr = attackRange * attackRange;
    }

    private void OldUpdate()
    {
        if(lane == null) return;

        if (GameManager.Instance.inCombat && pause == false)
        {
            //HandleCombat();
        }
        else
        {
            //HandleWander();
        }
    }

    public void Fill(UnitData data, Lane lane, bool playerControlled, Vector2 spawnPoint)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        attackSpeed = data.attackSpeed;
        moveSpeed = data.moveSpeed;
        spriteRenderer.sprite = data.sprite;

        attackRangeSqr = attackRange * attackRange;

        this.playerControlled = playerControlled;
        spriteRenderer.color = playerControlled ? normalColor : enemyColor;
        
        this.lane = lane;

        transform.position = spawnPoint;
        this.spawnPosition = spawnPoint;

        PickNewTarget();
        retargetTimer = Random.Range(0f, 0.15f);

        nearbyUnits = new Collider2D[32];
    }

    public void SetLane(Lane lane)
    {
        this.lane = lane;
    }
    #region // Old
    
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

    public void HandleCombat(float deltaTime)
    {
        attackCooldown -= deltaTime;
        retargetTimer -= deltaTime;

        Vector2 myPos = cachedTransform.position;

        if (retargetTimer <= 0f)
        {
           if (currentTarget == null || !currentTarget.gameObject.activeSelf)
            {
                currentTarget = FindClosestEnemy();
            }
            else
            {
                Vector2 offset =
                    (Vector2)currentTarget.cachedTransform.position - myPos;

                if (offset.sqrMagnitude > attackRangeSqr)
                {
                    currentTarget = FindClosestEnemy();
                }
            }

            retargetTimer = 0.15f;
        }

        if (currentTarget == null)
            return;

        Vector2 targetOffset =
            (Vector2)currentTarget.cachedTransform.position - myPos;

        float sqrDist = targetOffset.sqrMagnitude;

        if (sqrDist <= attackRangeSqr)
        {
            AttackTarget();
        }
        else
        {
            MoveTowardsTarget(deltaTime, targetOffset, sqrDist);
        }
    }

    ArmyUnit FindClosestEnemy()
    {
        ArmyUnit closest = null;
        float closestSqrDist = Mathf.Infinity;

        Vector2 myPos = transform.position;

        
        foreach (var unit in lane.GetArmy(!playerControlled))
        {
            if (unit == null) continue;
            if (unit == this) continue;
            
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

    private void MoveTowardsTarget(float deltaTime, Vector2 direction, float sqrDist)
    {
        if (sqrDist <= 0.001f)
            return;

        direction /= Mathf.Sqrt(sqrDist);

        Vector2 separation = ComputeSeparation();

        Vector2 finalDirection = (direction + separation).normalized;

        cachedTransform.position +=
            (Vector3)(finalDirection * moveSpeed * deltaTime);
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

    #endregion

    /// -------------
    /// Experimental Code
    /// -------------

    #region
    /*
    [SerializeField] float cohesionStrength = 0.35f;
    [SerializeField] float alignmentStrength = 0.2f;
    [SerializeField] float frontlineResistance = 0.5f;
    [SerializeField] float engagementSlowMultiplier = 0.15f;


    private Vector2 currentVelocity;
    //private List<ArmyUnit> cachedEnemyArmy;

    public int currentAttackers = 0;

    private struct SteeringData
    {
        public Vector2 separation;
        public Vector2 cohesion;
        public Vector2 alignment;
        public float localDensity;
    }

    public void HandleCombat(float deltaTime)
    {
        attackCooldown -= deltaTime;
        retargetTimer -= deltaTime;

        Vector2 myPos = cachedTransform.position;

        // ----------------------------
        // TARGETING
        // ----------------------------

        if (retargetTimer <= 0f)
        {
            if (
                currentTarget == null ||
                !currentTarget.gameObject.activeSelf
            )
            {
                SetTarget(FindBestEnemy());
            }

            retargetTimer = Random.Range(0.2f, 0.45f);
        }

        if (currentTarget == null)
            return;

        Vector2 targetOffset =
            (Vector2)currentTarget.cachedTransform.position - myPos;

        float sqrDist = targetOffset.sqrMagnitude;

        bool inRange = sqrDist <= attackRangeSqr;

        // ----------------------------
        // FRONTLINE LOCKING
        // ----------------------------

        if (inRange)
        {
            AttackTarget();

            // Hold position more while fighting
            if (Random.value < engagementSlowMultiplier)
            {
                MoveTowardsTarget(
                    deltaTime,
                    targetOffset,
                    sqrDist,
                    true
                );
            }

            return;
        }

        MoveTowardsTarget(
            deltaTime,
            targetOffset,
            sqrDist,
            false
        );
    }

    private void SetTarget(ArmyUnit newTarget)
    {
        if (currentTarget != null)
        {
            currentTarget.currentAttackers--;
        }

        currentTarget = newTarget;

        if (currentTarget != null)
        {
            currentTarget.currentAttackers++;
        }
    }

    ArmyUnit FindBestEnemy()
    {
        ArmyUnit best = null;

        float bestScore = Mathf.Infinity;

        Vector2 myPos = cachedTransform.position;

        List<ArmyUnit> enemies =
            lane.GetArmy(!playerControlled);

        for (int i = 0; i < enemies.Count; i++)
        {
            ArmyUnit unit = enemies[i];

            if (unit == null)
                continue;

            if (!unit.gameObject.activeSelf)
                continue;

            Vector2 offset =
                (Vector2)unit.cachedTransform.position - myPos;

            float sqrDist = offset.sqrMagnitude;

            // ----------------------------
            // TARGET SATURATION
            // ----------------------------

            float attackerPenalty =
                unit.currentAttackers * 2f;

            // Prefer enemies already engaged
            float frontlineBonus =
                unit.currentAttackers > 0 ? -1.5f : 0f;

            float score =
                sqrDist +
                attackerPenalty +
                frontlineBonus;

            if (score < bestScore)
            {
                bestScore = score;
                best = unit;
            }
        }

        return best;
    }

    private SteeringData ComputeSteering()
    {
        SteeringData data = new SteeringData();

        Vector2 myPos = cachedTransform.position;

        int allyCount = 0;

        int hits = Physics2D.OverlapCircleNonAlloc(
            myPos,
            separationRadius,
            nearbyUnits
        );

        for (int i = 0; i < hits; i++)
        {
            ArmyUnit unit =
                nearbyUnits[i].GetComponent<ArmyUnit>();

            if (unit == null || unit == this)
                continue;

            if (unit.playerControlled != playerControlled)
                continue;

            Vector2 offset =
                myPos -
                (Vector2)unit.cachedTransform.position;

            float sqrDist = offset.sqrMagnitude;

            if (sqrDist < 0.0001f)
                continue;

            float dist = Mathf.Sqrt(sqrDist);

            Vector2 dir = offset / dist;

            // ----------------------------
            // SEPARATION
            // ----------------------------

            data.separation += dir / dist;

            // ----------------------------
            // COHESION
            // ----------------------------

            data.cohesion +=
                (Vector2)unit.cachedTransform.position;

            // ----------------------------
            // ALIGNMENT
            // ----------------------------

            data.alignment += unit.currentVelocity;

            allyCount++;

            // ----------------------------
            // LOCAL DENSITY
            // ----------------------------

            if (Vector2.Dot(-dir, currentVelocity.normalized) > 0.5f)
            {
                data.localDensity += 1f;
            }
        }

        if (allyCount > 0)
        {
            data.separation /=
                allyCount;

            data.cohesion =
                ((data.cohesion / allyCount) - myPos)
                * cohesionStrength;

            data.alignment =
                (data.alignment / allyCount)
                * alignmentStrength;
        }

        data.separation *= separationStrength;

        return data;
    }

    private void MoveTowardsTarget(
        float deltaTime,
        Vector2 direction,
        float sqrDist,
        bool engaged)
    {
        if (sqrDist <= 0.001f)
            return;

        float dist = Mathf.Sqrt(sqrDist);

        direction /= dist;

        SteeringData steering =
            ComputeSteering();

        // ----------------------------
        // TARGET FORCE
        // ----------------------------

        Vector2 desiredDirection =
            direction;

        // ----------------------------
        // COMBINE STEERING
        // ----------------------------

        desiredDirection += steering.separation;
        desiredDirection += steering.cohesion;
        desiredDirection += steering.alignment;

        // ----------------------------
        // FRONTLINE PRESSURE
        // ----------------------------

        float densitySlow =
            1f /
            (1f + steering.localDensity * frontlineResistance);

        // ----------------------------
        // ENGAGEMENT LOCKING
        // ----------------------------

        float engageMultiplier =
            engaged ? engagementSlowMultiplier : 1f;

        desiredDirection.Normalize();

        currentVelocity =
            desiredDirection *
            moveSpeed *
            densitySlow *
            engageMultiplier;

        cachedTransform.position +=
            (Vector3)(currentVelocity * deltaTime);
    }

    void AttackTarget()
    {
        if (attackCooldown > 0f)
            return;

        attackCooldown = 1f / attackSpeed;

        if (currentTarget == null)
            return;

        if (!currentTarget.gameObject.activeSelf)
            return;

        if (attackType == AttackType.Melee)
        {
            currentTarget.TakeDamage(attackDamage);
        }
        else if (attackType == AttackType.Ranged)
        {
            currentTarget.TakeDamage(attackDamage);
        }
    }
    */
    #endregion

    public void TakeDamage(float amount)
    {
        if (!gameObject.activeSelf) return;
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        lane.RemoveFromArmy(this);
        UnitPool.Instance.ReleaseFrom(this);
        //Destroy(gameObject);
    }

    #region Wandering code

    public void HandleWander(float deltaTime)
    {
        if (isWaiting)
        {
            waitTime -= deltaTime;
            if (waitTime <= 0) 
            {
                PickNewTarget();
                isWaiting = false;
            }
        }
        else
        {
            MoveToTarget(deltaTime);
        }
    }

    private void MoveToTarget(float deltaTime)
    {
        Vector2 toTarget = (targetPosition - (Vector2)transform.position).normalized;

        //Vector2 separation = ComputeSeparation();
        Vector2 separation = Vector2.zero;

        Vector2 finalDirection = (toTarget + separation).normalized;

        transform.position += (Vector3)(finalDirection * moveSpeed * deltaTime);

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
