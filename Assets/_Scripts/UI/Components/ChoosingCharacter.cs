using UnityEngine;

public class ChoosingCharacter : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _class;
    [SerializeField] private ScriptablePlayerStat _playerStat;

    public void OnChoosingCharacter()
    {
        if (_class != null)
        {
            _DestroyContainerAllChildren();
            Instantiate(_class, _container.transform);
            if (CreatingCharacterSceneUI.Instance != null)
            {
                CreatingCharacterSceneUI.Instance.Player = _playerStat;
            }
        }
    }

    private void _DestroyContainerAllChildren()
    {
        var containerTransform = _container.transform;
        foreach (Transform child in containerTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
