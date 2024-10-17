using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoPopup : Singleton<SkillInfoPopup>
{
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _describe;
    [SerializeField] private TextMeshProUGUI _manaConsumed;
    [SerializeField] private MoveTo _content;
    [SerializeField] private GameObject _background;
    [SerializeField] private Vector3 _contentStartPosition;
    private Action _onUsed;
    private bool _showing = false;

    public bool Showing => _showing;

    private void Start()
    {
        _contentStartPosition = _content.transform.localPosition;
    }

    public SkillInfoPopup SetSkillName(string value)
    {
        _skillName.text = value;
        return this;
    }

    public SkillInfoPopup SetSkillImage(Sprite value)
    {
        _skillImage.sprite = value;
        return this;
    }

    public SkillInfoPopup SetSkillDescribe(string value)
    {
        _describe.text = value;
        return this;
    }

    public SkillInfoPopup SetManaConsumed(float value)
    {
        _manaConsumed.text = value.ToString();
        return this;
    }

    public SkillInfoPopup SetOnUsed(Action value)
    {
        _onUsed = value;
        return this;
    }

    public void Show()
    {
        GameManager.Instance?.SetPausing(true);
        _background?.SetActive(true);
        _content.To = Vector3.zero;
        _showing = true;
    }

    public void Hide()
    {
        GameManager.Instance?.SetPausing(false);
        _background?.SetActive(false);
        _content.To = _contentStartPosition;
        _showing = false;
    }

    public void OnUseButtonClick()
    {
        _onUsed?.Invoke();

        AudioManager.Instance?.PlayButton();
        Hide();
    }

    public void OnCloseButtonClick() => Hide();

}
