using System;
using UnityEngine;

public class SkillTreePopup : PopupTemplate
{
    [SerializeField] private SkillTreeElement _skill1;
    [SerializeField] private SkillTreeElement _skill2;
    [SerializeField] private SkillTreeElement _skill3;

    private void Start()
    {
        _LoadSkills();
    }

    public void OnCloseButtonClicked()
    {
        HidePopup();
    }

    private void _LoadSkills()
    {
        var choosingLevelUnitManager = ChoosingLevelUnitManager.Instance;
        if (choosingLevelUnitManager == null)
        {
            Debug.LogWarning("[SkillTreePopup] - ChoosingLevelUnitManager is null");
            return;
        }

        var playerData = choosingLevelUnitManager.PlayerData;
        if (playerData == null)
        {
            Debug.LogWarning("[SkillTreePopup] - choosingLevelUnitManager.PlayerData is null");
            return;
        }

        try
        {
            _skill1.SkillData = playerData.SkillData[0];
            _skill2.SkillData = playerData.SkillData[1];
            _skill3.SkillData = playerData.SkillData[2];
        }
        catch (Exception e)
        {
            Debug.Log($"[SkillTreePopup] - {e}");
        }

    }
}
