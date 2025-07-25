using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Player player = GetComponent<Player>();
            player.TakeDamage(20);
        }
        else if (collision.CompareTag("Usb"))
        {
            Debug.Log("USB collected! You win!");
        }
    }
}