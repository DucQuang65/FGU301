using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{   // Public variables
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected Player player;
    [SerializeField] protected MainTower tower;
    protected float currentHp;
    [SerializeField] protected float maxHp = 50f;
    [SerializeField] private Image hpBar;
    [SerializeField] private float playerChaseRange = 5f;
    [SerializeField] protected float enterDamage = 10f;
    [SerializeField] protected float stayDamage = 1f;
    protected enum EnemyState
    {
        ChasingTower,
        ChasingPlayer
    }
    protected EnemyState currentState = EnemyState.ChasingTower;
    protected virtual void Awake()
    {
        // Initialize references if not assigned in Inspector
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            if (player == null)
            {
                Debug.LogError("Player not found in scene!", this);
            }
        }

        if (tower == null)
        {
            tower = FindFirstObjectByType<MainTower>();
            if (tower == null)
            {
                Debug.LogError("MainTower not found in scene!", this);
            }
        }
    }
    protected virtual void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not found in the scene!"); // Warn forgot to assign the reference
        }
        currentHp = maxHp; // Initialize current HP to max HP
        UpdateHpBar();
    }
    protected virtual void Update()
    {
        if (tower == null)
        {
            Debug.LogWarning("Cannot move: Tower is missing.", this);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Check the distance to the player
        if (player != null && distanceToPlayer <= playerChaseRange)
        {
            currentState = EnemyState.ChasingPlayer;
        }
        else
        {
            currentState = EnemyState.ChasingTower;
        }

        // Action base on current state
        switch (currentState)
        {
            case EnemyState.ChasingPlayer:
                MoveToPlayer();
                break;
            case EnemyState.ChasingTower:
                MoveToTower();
                break;
        }
    }

    protected void MoveToPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= playerChaseRange)
            {
                Vector2 newPosition = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                FlipToTarget(player.transform);
            }
        }
    }
    protected void MoveToTower()
    {
        if (tower != null)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, tower.transform.position, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            FlipToTarget(tower.transform);
        }
    }

    protected void FlipToTarget(Transform target)
    {
        if (target == null) return;

        Vector3 scale = transform.localScale;
        if (target.position.x < transform.position.x)
        {
            scale.x = -Mathf.Abs(scale.x); // flip to left
        }else{
            scale.x = Mathf.Abs(scale.x);  // flip to right
        }
        transform.localScale = scale;
    }

    public virtual void TakeDamage(float damage)
    {
        // Handle enemy taking damage
        Debug.Log($"Enemy took {damage} damage!");
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0); // Ensure current HP does not go below 0
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        // Handle enemy death
        Destroy(gameObject); // Destroy the enemy game object
    }
    protected void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp; // Update the HP bar fill amount
        }
    }
}
