using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Projectile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public float speed;

    [SerializeField] private float arriveDist = 0.08f;

    private float damage;
    
    private ArmyUnit targetUnit;
    private Transform targetTransform;
    private Transform cachedTransform;
    private float arriveDistSqr;

    private void Awake()
    {
        cachedTransform = transform;
        arriveDistSqr = arriveDist * arriveDist;
    }

    // Update is called once per frame
    private void Update()
    {
        if (targetTransform != null) 
        {
            if (!targetUnit.gameObject.activeSelf)
            {
                Release();
                return;
            }

            Vector2 myPos = cachedTransform.position;

            Vector2 targetOffset = (Vector2)targetTransform.position - myPos;

            Vector2 direction = targetOffset.normalized;

            cachedTransform.position += (Vector3)(direction * speed * Time.deltaTime);
            // Rotate toward target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            cachedTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (targetOffset.sqrMagnitude <= arriveDistSqr)
            {
                HitTarget();
            }

        }
    }

    public void Fill(Vector2 position, ArmyUnit target, float damage, float speed, Sprite sprite)
    {
        cachedTransform.position = position;
        targetUnit = target;
        targetTransform = targetUnit.transform;
        this.damage = damage;
        this.speed = speed;
        spriteRenderer.sprite = sprite;
    }

    private void HitTarget()
    {
        targetUnit.TakeDamage(damage);
        Release();
    }

    private void Release()
    {
        UnitPool.Instance.ReleaseFromProjectile(this);
    }

}
