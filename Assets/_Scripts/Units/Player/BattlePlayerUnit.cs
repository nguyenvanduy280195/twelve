using System.Collections;
using UnityEngine;

public class BattlePlayerUnit : BattleUnitBase
{
    [SerializeField] private PlayerData _stat;

    public bool ExecutingSkill { set; private get; } = false;

    private bool IsDraggingSuccess(GameObject go) => IsDraggingSuccess(go.GetComponent<Item>());

    private bool IsDraggingSuccess(Item item)
    {
        var gameManager = BattleGameManager.Instance;
        bool duplicated = item.IsDuplicatedWith(gameManager.ItemSelected);
        bool neighbored = item.IsNeighborWith(gameManager.ItemSelected);
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
                gameManager.ItemSelected = hit.collider.gameObject;

            }
        }

        if (Input.GetMouseButton(0) && gameManager.ItemSelected != null)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
            {
                gameManager.ItemDragged = hit.collider.gameObject;
                gameManager.PreSelectedPosition = gameManager.ItemSelected.transform.position;
                gameManager.PreDraggedPosition = gameManager.ItemDragged.transform.position;
                return true;
            }
        }

        return false;
    }

    public override IEnumerator ControlCoroutine()
    {
        var gameManager = BattleGameManager.Instance;


        gameManager.ItemSelected = null;
        gameManager.ItemDragged = null;

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    gameManager.ItemSelected = hit.collider.gameObject;
                }
            }

            if (Input.GetMouseButton(0) && gameManager.ItemSelected != null)
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
                {
                    gameManager.ItemDragged = hit.collider.gameObject;
                    gameManager.PreSelectedPosition = gameManager.ItemSelected.transform.position;
                    gameManager.PreDraggedPosition = gameManager.ItemDragged.transform.position;
                    break;
                }
            }

            if (ExecutingSkill)
            {
                ExecutingSkill = false;
                break;
            }

            yield return null;
        }
    }


    public override void IncreaseGold(float bonusFactor) => nGold += (1 + 0.01f * _stat.Luck) * bonusFactor;

    public override void IncreaseExp(float bonusFactor) => nExp += (1 + 0.01f * _stat.Luck) * bonusFactor;

    protected override UIUnit UIUnit => BattleUIManager.Instance.Player;

    protected override Vector3 UnitAttackPosition => BattleUnitManager.Instance.PlayerAttackPosition;

    public override UnitData Stat
    {
        get => _stat;
        set
        {
            _stat = (PlayerData)value;
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

    public PlayerData PlayerStat => Stat as PlayerData;
}
