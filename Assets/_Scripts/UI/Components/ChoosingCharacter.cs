using UnityEngine;

public class ChoosingCharacter : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _class;
    [SerializeField] private ScriptablePlayerData _scriptablePlayerData;

    public void OnChoosingCharacter()
    {
        if (_class != null)
        {
            _DestroyContainerAllChildren();
            Instantiate(_class, _container.transform);
            CreatingCharacterSceneUI.Instance?.SetPlayerData(_scriptablePlayerData.PlayerData);
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
