using UnityEngine;

public class HealEnemy : Enemy
{
    [SerializeField] private float healAmount = 20f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(enterDamage);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(stayDamage);
            }
        }
    }
    protected override void Die()
    {   
        HealPlayer(); // Heal the player before dying
        base.Die();
    }
    private void HealPlayer()
    {
        if (player != null)
        {
            player.Heal(healAmount); // Heal the player
        }
    }

}
