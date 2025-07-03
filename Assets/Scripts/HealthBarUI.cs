using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private MainTower mainTower;   // Kéo MainTower vào đây
    [SerializeField] private Image fillImage;       // Kéo Fill (màu đỏ) vào đây
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill = 1f;

    private void Start()
    {
        if (mainTower != null)
        {
            mainTower.onHealthChanged.AddListener(UpdateHealthTarget);
            UpdateHealthTarget();
        }
        else
        {
            Debug.LogError("MainTower chưa được gán vào HealthBarUI!");
        }
    }

    private void Update()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
        }
    }

    private void UpdateHealthTarget()
    {
        float current = mainTower.GetCurrentHP();
        float max = mainTower.GetMaxHP();
        targetFill = current / max;
    }
}
