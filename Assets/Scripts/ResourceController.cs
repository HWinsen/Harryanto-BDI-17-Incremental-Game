using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private Button _ResourceButton;
    public Image ResourceImage;
    [SerializeField] private Text _ResourceDescription;
    [SerializeField] private Text _ResourceUpgradeCost;
    [SerializeField] private Text _ResourceUnlockCost;
    
    private ResourceConfig _config;

    private int _level = 1;

    public bool IsUnlocked {get; private set;}

    private void Start()
    {
        _ResourceButton.onClick.AddListener(() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    public void SetConfig(ResourceConfig config)
    {
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        _ResourceDescription.text = $"{_config.Name} Lv. {_level}\n+{GetOutput().ToString("0")}";
        _ResourceUnlockCost.text = $"Unlock Cost\n{_config.UnlockCost}";
        _ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";

        SetUnlocked(_config.UnlockCost == 0);
    }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel()
    {
        // Debug.Log("pencet");
        double upgradeCost = GetUpgradeCost();
        if (GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        _ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        _ResourceDescription.text = $"{_config.Name} Lv. {_level}\n{GetOutput().ToString("0")}";
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }

        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();
        AchievementController.instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
    }

    public void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        _ResourceUnlockCost.gameObject.SetActive(!unlocked);
        _ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
