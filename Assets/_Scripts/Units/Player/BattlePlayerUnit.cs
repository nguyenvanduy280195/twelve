using System.Collections;
using UnityEngine;

public class BattlePlayerUnit : BattleUnitBase
{
    [SerializeField] private PlayerStat _stat;

    private bool IsDraggingSuccess(GameObject go) => IsDraggingSuccess(go.GetComponent<Item>());

    private bool IsDraggingSuccess(Item item)
    {
        var gameManager = BattleGameManager.Instance;
        bool duplicated = item.IsDuplicatedWith(gameManager.SelectedGameObject);
        bool neighbored = item.IsNeighborWith(gameManager.SelectedGameObject);
        return !duplicated && neighbored;
    }

    public override bool Control()
    {
        var gameManager = BattleGameManager.Instance;

        if (Input.GetMouseButtonDown(0))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                gameManager.SelectedGameObject = hit.collider.gameObject;
            }
        }

        if (Input.GetMouseButton(0) && gameManager.SelectedGameObject != null)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
            {
                gameManager.DraggedGameObject = hit.collider.gameObject;
                gameManager.PreSelectedPosition = gameManager.SelectedGameObject.transform.position;
                gameManager.PreDraggedPosition = gameManager.DraggedGameObject.transform.position;
                return true;
            }
        }

        return false;
    }

    public override IEnumerator ControlCoroutine()
    {
        while (true)
        {
            var gameManager = BattleGameManager.Instance;

            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    gameManager.SelectedGameObject = hit.collider.gameObject;
                }
            }

            if (Input.GetMouseButton(0) && gameManager.SelectedGameObject != null)
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
                {
                    gameManager.DraggedGameObject = hit.collider.gameObject;
                    gameManager.PreSelectedPosition = gameManager.SelectedGameObject.transform.position;
                    gameManager.PreDraggedPosition = gameManager.DraggedGameObject.transform.position;
                    break;
                }
            }
            yield return null;
        }
    }


    public override void IncreaseGold(float bonusFactor) => nGold += (1 + 0.01f * _stat.Luck) * bonusFactor;

    public override void IncreaseExp(float bonusFactor) => nExp += (1 + 0.01f * _stat.Luck) * bonusFactor;

    protected override UIUnit UIUnit => BattleUIManager.Instance.Player;

    protected override Vector3 UnitAttackPosition => BattleUnitManager.Instance.PlayerAttackPosition;

    public override UnitStat Stat
    {
        get => _stat;
        set
        {
            _stat = (PlayerStat)value;
            _InitializeUIUnit();
        }
    }
    protected override void _InitializeUIUnit()
    {
        base._InitializeUIUnit();

        UIUnit.HP.Value = PlayerStat.HP;
        UIUnit.Mana.Value = PlayerStat.Mana;
        UIUnit.Stamina.Value = PlayerStat.Stamina;
    }
    protected override float HP
    {
        set
        {
            base.HP = value;
            PlayerStat.HP = value;
        }
    }

    protected override float Mana
    {
        set
        {
            base.Mana = value;
            PlayerStat.Mana = value;
        }
    }

    protected override float Stamina
    {
        set
        {
            base.Stamina = value;
            PlayerStat.Stamina = value;
        }
    }

    public PlayerStat PlayerStat => Stat as PlayerStat;
}
