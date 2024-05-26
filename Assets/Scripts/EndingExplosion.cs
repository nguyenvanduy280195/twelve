using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingExplosion : MonoBehaviour
{
    private Leader _leader;

    private void Awake()
    {
        _leader = GetComponentInParent<Leader>();
    }

    private void OnDestroy()
    {
        if(_leader != null)
        {
            _leader.ExplosionAnimations.Remove(gameObject);
        }
    }

}
