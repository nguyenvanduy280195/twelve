using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingExplosion : MonoBehaviour
{
    private void OnDestroy() => BattleGameManager.Instance.ExplosionAnimations.Remove(gameObject);
}
