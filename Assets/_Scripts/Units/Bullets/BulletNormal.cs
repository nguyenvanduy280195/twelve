
using System.Collections;
using UnityEngine;

public class BulletNormal : MonoBehaviour
{
    private float _damage;
    private float _speed;
    private BattleUnitBase _targetUnit;

    public BulletNormal SetDamage(float damage)
    {
        _damage = damage;
        return this;
    }

    public BulletNormal SetSpeed(float speed)
    {
        _speed = speed;
        return this;
    }
    public BulletNormal SetTargetUnit(BattleUnitBase unit)
    {
        _targetUnit = unit;
        return this;
    }

    public void Fire() => StartCoroutine(_Fire());

    private IEnumerator _Fire()
    {
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetUnit.transform.position, _speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _targetUnit.gameObject)
        {
            _targetUnit?.TakeHit(_damage);
            Destroy(gameObject);
        }
    }
}