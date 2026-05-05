using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyUnit : MonoBehaviour
{
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

    void Start()
    {
        spawnPosition = transform.position;
        PickNewTarget();
    }

    private void Update()
    {
        if (isWaiting) return;

        MoveToTarget();
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
            StartCoroutine(WaitAndPickNewTarget());
        }
    }

    private void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPosition + randomOffset;
    }

    private IEnumerator WaitAndPickNewTarget()
    {
        isWaiting = true;

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        PickNewTarget();
        isWaiting = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPosition : transform.position, wanderRadius);
    }
}
