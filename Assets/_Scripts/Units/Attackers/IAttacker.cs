using UnityEngine;
public interface IAttacker
{
    void Attack();
    IAttacker SetDamage(float damage);
    IAttacker SetMoveSpeed(float speed);
    IAttacker SetBulletSpeed(float speed);
    IAttacker SetTargetUnit(BattleUnitBase unit);
    IAttacker SetAttackPosition(Vector3 position);
}