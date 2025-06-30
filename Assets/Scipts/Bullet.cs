using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;

    public void Setup(Vector2 dir, float spd, float dmg)
    {
        Debug.Log($"Bullet khởi tạo: hướng={dir}, tốc độ={spd}, sát thương={dmg}");
        direction = dir;
        speed = spd;
        damage = dmg;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        Destroy(gameObject, 5f);
    }

    // Xử lý va chạm với các đối tượng enemy, hãy gắn enemy tag cho các đối tượng enemy trong Unity
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Bullet va chạm với: {other.gameObject.name}, tag={other.tag}, Collider={other}, position={transform.position}");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet trúng enemy tại {other.transform.position} với {damage} sát thương");
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}