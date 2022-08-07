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
            if (s_instance == null)
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
    [SerializeField] private Sprite[] _resourcesSprites;

    [SerializeField] private Transform _resourcesParent;
    [SerializeField] private ResourceController _resourcePrefab;
    [SerializeField] private TapText _tapTextPrefab;

    [SerializeField] private Transform _coinIcon;
    [SerializeField] private Text _goldInfo;
    [SerializeField] private Text _autoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;

    public double TotalGold {get; private set;}


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

        CheckResourceCost();

        _coinIcon.transform.localScale = Vector3.LerpUnclamped(_coinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        _coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
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

    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isBuyable = TotalGold >= resource.GetUpgradeCost();

            resource.ResourceImage.sprite = _resourcesSprites[isBuyable ? 1:0];
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

    public void AddGold(double value)
    {
        TotalGold += value;
        _goldInfo.text = $"Gold: {TotalGold.ToString("0")}";
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double _output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            _output += resource.GetOutput();
        }

        TapText _tapText = GetOrCreateTapText();
        _tapText.transform.SetParent(parent, false);
        _tapText.transform.position = tapPosition;
        
        _tapText.Text.text = $"+{_output.ToString("0")}";
        _tapText.gameObject.SetActive(true);
        _coinIcon.transform.localScale = Vector3.one * 1.75f;
        
        AddGold(_output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText _tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if (_tapText == null)
        {
            _tapText = Instantiate(_tapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(_tapText);
        }

        return _tapText;
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