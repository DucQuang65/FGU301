using Unity.VisualScripting;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float speedShootBullet = 20f;
    [SerializeField] private float speedCircularShot = 10f;
    [SerializeField] private float hpValue = 20f;
    [SerializeField] private GameObject Mions;
    [SerializeField] private float skillCooldown = 5f;
    [SerializeField] private GameObject usbPrefab;
    
    private float nextSkillTime = 0f;
 
    private bool isPlayerInRange; // Track if Player is in trigger

    protected override void Awake()
    {
        base.Awake();
        enterDamage = 20f;
        stayDamage = 10f;
        moveSpeed = 2.5f;
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= nextSkillTime)
        {
            UseSkill();
        }
        // Move toward Tower if Player is not in range
        if (!isPlayerInRange && tower != null)
        {
            Vector2 direction = (tower.transform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            Debug.Log("Moving toward Tower!");
        }
    }
    protected override void Die()
    {
        Instantiate(usbPrefab, transform.position, Quaternion.identity); // Spawn USB on death
        base.Die(); // Call the base class Die method to handle the death logic
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prioritize Player
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // Stop moving toward Tower
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(enterDamage);
                Debug.Log($"Dealt {enterDamage} damage to Player on enter!");
            }
        }
        // Attack Tower if no Player
        else if (collision.CompareTag("Tower"))
        {
            MainTower tower = collision.GetComponent<MainTower>();
            if (tower != null)
            {
                tower.TakeDamage(enterDamage);
                Debug.Log($"Dealt {enterDamage} damage to Tower on enter!");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Continue attacking Player while in range
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(stayDamage * Time.deltaTime); // Scale damage by time
                Debug.Log($"Dealing {stayDamage} damage/sec to Player on stay!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Resume targeting Tower when Player leaves range
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left range, resuming Tower target!");
        }
    }
    private void Teleport()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
    private void SpawnMions()
    {
        Instantiate(Mions, transform.position, Quaternion.identity);
        Debug.Log("Spawning minions!");
    }
    private void ShootBullet()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - firePoint.position;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetMovementDirection(directionToPlayer * speedShootBullet);
        }
        Debug.Log("Shooting bullet!");
    }
    private void CircularShot()
    {
        const int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad; // Convert angle to radians
            Vector3 bulletDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetMovementDirection(bulletDirection * speedCircularShot);
        }
        Debug.Log("Performing circular shot!");
    }
    private void Healing(float hpAmount)
    {
        currentHp = Mathf.Min(currentHp + hpAmount, maxHp); // Heal the boss by 20, but not exceeding max HP
        UpdateHpBar();
        Debug.Log("Healing boss!");
    }
    private void RandomSkill()
    {
        int ranedomSkill = Random.Range(0, 4);
        switch (ranedomSkill)
        {
            case 0:
                Teleport();
                Debug.Log("Teleporting!");
                break;
            case 1:
                ShootBullet();
                Debug.Log("Shooting bullet!");
                break;
            case 2:
                CircularShot();
                Debug.Log("Performing circular shot!");
                break;
            case 3:
                Healing(hpValue);
                Debug.Log("Healing boss!");
                break;
            case 4:
                SpawnMions();
                Debug.Log("Spawning minions!");
                break;
        }
    }
    private void UseSkill()
    {
        nextSkillTime = Time.time + skillCooldown; // Set the next skill time
        RandomSkill(); // Call the random skill method
    }
}
