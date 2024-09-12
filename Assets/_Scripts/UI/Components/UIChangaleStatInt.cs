using System;
using UnityEngine;

public class UIChangaleStatInt : UIStatInt
{
    public static event Action<int> OnNumberOfPointsChanged;
    public static event Func<int> OnNumberOfPointsGot;
    [SerializeField] private GameObject _downButton;
    [SerializeField] private GameObject _upButton;
    protected int _threshDown = -1;
    public bool UpButtonEnabled { set => _upButton.SetActive(value); }
    public bool DownButtonEnabled { set => _downButton.SetActive(value); }
    public int Weight = 1;
    public override int Content
    {
        set
        {
            base.Content = value;

            if (_threshDown <= -1)
            {
                _threshDown = value;
            }
        }
    }

    public void OnIncrease()
    {

        if (OnNumberOfPointsGot?.Invoke() > 0)
        {
            OnNumberOfPointsChanged?.Invoke(1);
            Content += Weight;
        }
        AudioManager.Instance?.PlayButton();
    }

    public void OnDecrease()
    {
        if (Content > _threshDown)
        {
            Content -= Weight;
            OnNumberOfPointsChanged?.Invoke(-1);
        }
        AudioManager.Instance?.PlayButton();
    }
}
