using UnityEngine;

public class ContactHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if(collision.CompareTag("Enemy"))
        {
            MatchingBattleManager.Instance.BeginBattle(collision.gameObject);
        }
    }
}
