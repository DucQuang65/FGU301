using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject mainTower;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelUpButton;
    private MainTower tower;
    private float testGold = 1000f;
    private AudioSource audioSource; // Thêm AudioSource cho UI

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource
        if (audioSource == null)
        {
            Debug.LogError("AudioSource không được gắn trên UpgradeCanvas!");
        }

        if (mainTower != null)
        {
            tower = mainTower.GetComponent<MainTower>();
            tower.onTowerClicked.AddListener(OnTowerClicked);
            Debug.Log("Tower clicked listener added");
        }
        else
        {
            Debug.LogError("MainTower không tìm thấy!");
        }

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
            Debug.Log("UpgradePanel initialized");
        }
        if (statsPanel != null)
        {
            statsPanel.SetActive(true);
            Debug.Log("StatsPanel initialized");
        }

        if (levelUpButton != null)
            levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);

        UpdateUI();
    }

    private void OnTowerClicked()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            Debug.Log("UpgradePanel activated");
            UpdateUI();
        }
    }

    private void OnLevelUpButtonClicked()
    {
        if (tower != null && upgradePanel != null)
        {
            float upgradeCost = tower.GetUpgradeCost();
            if (!tower.IsMaxLevel() && testGold >= upgradeCost)
            {
                if (tower.Upgrade(testGold))
                {
                    testGold -= upgradeCost;
                    if (testGold < 0) testGold = 0;
                    Debug.Log($"Nâng cấp thành công! Còn {testGold} vàng");
                    UpdateUI();
                    if (audioSource != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip); // Phát âm thanh nhấp
                        Debug.Log("Playing click sound");
                    }
                }
            }
            else
            {
                Debug.Log("Không đủ vàng hoặc max level");
            }
        }
    }

    private void UpdateUI()
    {
        if (tower != null)
        {
            if (upgradeCostText != null)
                upgradeCostText.text = tower.IsMaxLevel() ? "Max Level" : $"Upgrade Cost: {tower.GetUpgradeCost()}";
            if (hpText != null)
                hpText.text = $"HP: {tower.GetCurrentHP()}/{tower.GetMaxHP()}";
            if (armorText != null)
                armorText.text = $"Armor: {tower.GetArmor()}";
            if (levelText != null)
                levelText.text = $"Level: {tower.GetLevel()}";
            if (levelUpButton != null)
                levelUpButton.interactable = !tower.IsMaxLevel() && testGold >= tower.GetUpgradeCost();
        }
    }
}