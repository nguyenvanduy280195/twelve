using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLoader : MonoBehaviour
{
    [SerializeField] private GameObject _meditatePrefab;
    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private GameObject _meteorPrefab;
    [SerializeField] private GameObject _powerStrikePrefab;
    [SerializeField] private GameObject _spearSpinPrefab;
    [SerializeField] private GameObject _multiAttackPrefab;


    private void Start()
    {
        StartCoroutine(_Start());
    }

    private IEnumerator _Start()
    {
        yield return new WaitUntil(() => BattleUnitManager.Instance != null);
        yield return new WaitUntil(() => BattleUnitManager.Instance.Player != null);

        var playerUnit = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        switch (playerUnit.Stat.Class)
        {
            case UnitClass.Soldier:
                Instantiate(_powerStrikePrefab, transform);
                Instantiate(_spearSpinPrefab, transform);
                Instantiate(_multiAttackPrefab, transform);
                break;
            case UnitClass.FireMage:
                Instantiate(_meditatePrefab, transform);
                Instantiate(_fireballPrefab, transform);
                Instantiate(_meteorPrefab, transform);
                break;
            default:
                Debug.LogWarning($"{playerUnit.Stat.Class} class's skills are implemented");
                break;
        }
    }

}
