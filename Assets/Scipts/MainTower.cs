using UnityEngine;
using UnityEngine.Events;

public class MainTower : MonoBehaviour
{
    [SerializeField] private float baseMaxHP = 1000f;
    [SerializeField] private float currentHP;
    [SerializeField] private float armor = 10f;
    [SerializeField] private int level = 1;
    [SerializeField] private float hpPerLevel = 200f;
    [SerializeField] private float armorPerLevel = 5f;
    [SerializeField] private float upgradeCostBase = 100f;
    [SerializeField] private float armorPerPoint = 0.04f;
    [SerializeField] private float maxDamageReduction = 0.8f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackInterval = 0.2f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float maxHP;
    [SerializeField] private AudioClip gunshotSound; 
    private const int maxLevel = 10;

    private float attackDamage = 20f;
    private float attackTimer = 0f;
    private bool isDestroyed = false;
    private AudioSource audioSource; 

    public UnityEvent onHealthChanged;
    public UnityEvent onLevelUp;
    public UnityEvent onDestroyed;
    public UnityEvent<int> onSubTowerUnlocked;
    public UnityEvent onTowerClicked;

    private void Start()
    {
        maxHP = baseMaxHP;
        currentHP = maxHP;
        audioSource = GetComponent<AudioSource>(); 
        if (audioSource == null)
        {
            Debug.LogError("AudioSource không được gắn trên MainTower!");
        }
        if (gunshotSound == null)
        {
            Debug.LogError("Gunshot sound chưa được gán!");
        }
        Debug.Log($"MainTower: HP={currentHP}/{maxHP}, Armor={armor}, Level={level}, Damage={attackDamage}");
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab chưa gán!");
        }
        onHealthChanged.Invoke();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    private void OnMouseDown()
    {
        onTowerClicked.Invoke();
    }


   
    public void TakeDamage(float damage, float pierceFactor = 0f)
    {
        float effectiveArmor = armor * (1f - Mathf.Clamp(pierceFactor, 0f, 1f));
        float damageReduction = effectiveArmor * armorPerPoint;
        damageReduction = Mathf.Clamp(damageReduction, 0f, maxDamageReduction);
        float actualDamage = damage * (1f - damageReduction);
        //trừ máu hiện tại theo sát thương đã tính toán
        currentHP -= actualDamage;
        currentHP = Mathf.Max(0f, currentHP); //ko cho âm
        Debug.Log($"TakeDamage: HP before={currentHP + actualDamage}, after={currentHP}");
        onHealthChanged.Invoke();

        //kiểm tra nếu HP <= 0 thì đánh dấu là đã bị phá hủy
        if (currentHP <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            Debug.Log("Game Over");
            onDestroyed.Invoke();
            gameObject.SetActive(false);
        }
    }

    //tấn công enemy gần nhất
    private void Attack()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;
            Debug.Log($"Bắn enemy tại {nearestEnemy.transform.position}, khoảng cách={Vector2.Distance(transform.position, nearestEnemy.transform.position)}, Damage={attackDamage}");
            Vector2 spawnPosition = (Vector2)transform.position + direction * 0.2f;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Setup(direction, bulletSpeed, attackDamage);
            }
            else
            {
                Debug.LogError("Script Bullet không tìm thấy!");
            }
            if (audioSource != null && gunshotSound != null)
            {
                audioSource.PlayOneShot(gunshotSound); 
            }
        }
    }

    //tìm enemy gần nhất 
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"Tìm thấy {enemies.Length} enemy với tag 'Enemy'");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    //upgrade level bằng tiền, ko đủ thì ko cho mua (có điều kiện level 2 trở lên mới mở khóa sub tower)
    public bool Upgrade(float availableGold)
    {
        if (level >= maxLevel)
            return false;

        float upgradeCost = upgradeCostBase * level * level;
        if (availableGold < upgradeCost)
            return false;

        level++;
        maxHP += hpPerLevel;
        armor += armorPerLevel;
        attackDamage += 1f;
        if (level >= 2)
        {
            int subTowerCount = level - 1;
            onSubTowerUnlocked.Invoke(subTowerCount);
            Debug.Log($"Mở khóa {subTowerCount} trụ phụ!");
        }

        onLevelUp.Invoke();
        onHealthChanged.Invoke();
        Debug.Log($"Nâng cấp! HP={currentHP}/{maxHP}, Armor={armor}, Level={level}, Damage={attackDamage}");
        return true;
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public float GetArmor() => armor;
    public int GetLevel() => level;
    public float GetUpgradeCost() => level >= maxLevel ? 0f : upgradeCostBase * level * level;
    public bool IsMaxLevel() => level >= maxLevel;
}