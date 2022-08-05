using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance = null;

    public static GameManager Instance
    {
        get
        {
            if (s_instance = null)
            {
                s_instance = FindObjectOfType<GameManager>();
            }

            return s_instance;
        }
    }

    // Fungsi [Range (min, max)] ialah menjaga value agar tetap berada di antara min dan max-nya
    [Range(0f, 1f)]
    [SerializeField] private float _autoCollectPercentage = 0.1f;
    [SerializeField] private ResourceConfig[] _resourcesConfigs;

    [SerializeField] private Transform _resourcesParent;
    [SerializeField] private ResourceController _resourcePrefab;

    [SerializeField] private Text _goldInfo;
    [SerializeField] private Text _autoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private float _collectSecond;

    private double _totalGold;


    // Start is called before the first frame update
    void Start()
    {
        AddAllResources();
    }

    // Update is called once per frame
    void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }
    }

    private void AddAllResources()
    {
        foreach (ResourceConfig config in _resourcesConfigs)
        {
            GameObject _obj = Instantiate(_resourcePrefab.gameObject, _resourcesParent, false);
            ResourceController _resource = _obj.GetComponent<ResourceController>();

            _resource.SetConfig(config);
            _activeResources.Add(_resource);
        }
    }

    private void CollectPerSecond()
    {
        double _output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            _output += resource.GetOutput();
        }

        _output *= _autoCollectPercentage;

        // Fungsi ToString("F1") ialah membulatkan angka menjadi desimal yang memiliki 1 angka di belakang koma 
        _autoCollectInfo.text = $"Auto Collect: {_output.ToString("F1")} / second";

        AddGold(_output);
    }

    private void AddGold(double value)
    {
        _totalGold += value;
        _goldInfo.text = $"Gold: {_totalGold.ToString("0")}";
    }
}

// Fungsi System.Serializable adalah agar object bisa di-serialize dan

// value dapat di-set dari inspector

[System.Serializable]

public struct ResourceConfig

{

    public string Name;

    public double UnlockCost;

    public double UpgradeCost;

    public double Output;

}