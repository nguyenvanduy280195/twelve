using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoPopup : PopupTemplate
{
    [SerializeField] TextMeshProUGUI _skillName;
    [SerializeField] Image _skillImage;
    [SerializeField] TextMeshProUGUI _describe;
    [SerializeField] TextMeshProUGUI _manaConsumed;

    public string SkillName { set => _skillName.text = value; }

    public Sprite SkillImage { set => _skillImage.sprite = value; }

    public string SkillDescribe { set => _describe.text = value; }

    public float ManaConsumed { set => _manaConsumed.text = value.ToString(); }

    public Action OnUsed;

    public void OnUseButtonClick()
    {
        OnUsed?.Invoke();

        AudioManager.Instance?.PlayButton();
        HidePopup();
    }

    public void OnCloseButtonClick() => HidePopup();
}
