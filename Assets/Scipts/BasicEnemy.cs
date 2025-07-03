using UnityEngine;

public class BasicEnemy : Enemy// Inherits from the Enemy class
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(enterDamage);
                Debug.Log("[BasicEnemy] Gây damage vào Player!");
            }
        }
        else if (collision.CompareTag("Tower"))
        {
            if (tower != null)
            {
                tower.TakeDamage(enterDamage);
                Debug.Log("[BasicEnemy] Gây damage vào MainTower!");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(stayDamage * Time.deltaTime);
                Debug.Log("[BasicEnemy] Gây damage liên tục vào Player!");
            }
        }
        else if (collision.CompareTag("Tower"))
        {
            if (tower != null)
            {
                tower.TakeDamage(stayDamage * Time.deltaTime);
                Debug.Log("[BasicEnemy] Gây damage liên tục vào MainTower!");
            }
        }
    }

}
