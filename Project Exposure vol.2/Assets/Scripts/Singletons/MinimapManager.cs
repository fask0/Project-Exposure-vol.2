using UnityEngine.UI;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    private const int TerrainWidth = 250;
    private const int TerrainLength = 500;
    private const int Radius = 100;

    [SerializeField] private Texture2D _undiscoverdMap;
    [SerializeField] private Texture2D _discoveredMap;

    private int _texWidth;
    private int _texHeight;
    private Vector2 _minimapSize;

    private RenderTexture _renderTex;
    private RectTransform _undiscoveredTransform;
    private Material _undiscoveredMaterial;
    private Texture2D _undiscoveredCopyTex;

    private RectTransform _discoveredTransform;
    private Material _discoveredMaterial;

    private Transform _player;
    private RectTransform _playerMarker;

    private float _zoom = 0.5f;

    void Start()
    {
        SingleTons.MinimapManager = this;

        _texWidth = _undiscoverdMap.width;
        _texHeight = _undiscoverdMap.height;
        _minimapSize = Camera.main.transform.GetChild(0).GetChild(10).GetComponent<RectTransform>().sizeDelta;

        _undiscoveredTransform = Camera.main.transform.GetChild(0).GetChild(10).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        _undiscoveredMaterial = Camera.main.transform.GetChild(0).GetChild(10).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().material;
        _undiscoveredCopyTex = new Texture2D(_texWidth, _texHeight, TextureFormat.RGBA32, false);
        _undiscoveredCopyTex.SetPixels(_undiscoverdMap.GetPixels());
        _undiscoveredCopyTex.Apply();

        _renderTex = new RenderTexture(_texHeight, _texHeight, 0, RenderTextureFormat.ARGB32);
        _undiscoveredMaterial.SetTexture("_MainTex", _undiscoveredCopyTex);
        _undiscoveredMaterial.SetFloat("_MainTexWidth", _texWidth);
        _undiscoveredMaterial.SetFloat("_MainTexHeight", _texHeight);
        _undiscoveredMaterial.SetFloat("_Radius", Radius);
        _undiscoveredMaterial.SetFloat("_RadiusSquared", Radius * Radius);

        _discoveredTransform = Camera.main.transform.GetChild(0).GetChild(10).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        _discoveredMaterial = Camera.main.transform.GetChild(0).GetChild(10).GetChild(0).GetChild(0).GetComponent<Image>().material;
        _discoveredMaterial.mainTexture = _discoveredMap;

        _player = SingleTons.GameController.Player.transform;
        _playerMarker = Camera.main.transform.GetChild(0).GetChild(10).GetChild(0).GetChild(1).GetComponent<RectTransform>();
    }

    void Update()
    {
        _playerMarker.localRotation = Quaternion.Euler(0, 0, -Camera.main.transform.eulerAngles.y);
        _playerMarker.localScale = new Vector3(_zoom, _zoom, 1);

        float relativeToWorldX = (_player.position.x / TerrainWidth) * _texWidth;
        float relativeToWorldZ = (_player.position.z / TerrainLength) * _texHeight;
        _discoveredTransform.localPosition = new Vector2((-relativeToWorldX - _minimapSize.x) * _zoom, (-relativeToWorldZ - _minimapSize.y) * _zoom);
        _discoveredTransform.localScale = new Vector3(_zoom, _zoom, 1);

        _undiscoveredMaterial.SetFloat("_PlayerPositionX", relativeToWorldX);
        _undiscoveredMaterial.SetFloat("_PlayerPositionY", relativeToWorldZ);

        _renderTex.Release();
        Graphics.Blit(_undiscoveredCopyTex, _renderTex, _undiscoveredMaterial);
        Graphics.CopyTexture(_renderTex, _undiscoveredCopyTex);
    }

    public void ZoomIn()
    {
        _zoom += 0.25f;
        _zoom = Mathf.Clamp(_zoom, 0.5f, 4);
    }

    public void ZoomOut()
    {
        _zoom -= 0.25f;
        _zoom = Mathf.Clamp(_zoom, 0.5f, 4);
    }

    private void OnDisable()
    {
        _playerMarker.localRotation = Quaternion.identity;
        _playerMarker.localScale = new Vector3(1, 1, 1);
        _discoveredTransform.localPosition = new Vector2(0, 0);
        _discoveredTransform.localScale = new Vector3(1, 1, 1);
    }
}
