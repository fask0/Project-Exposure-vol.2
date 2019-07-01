using UnityEngine.UI;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField] private int _radius;
    [SerializeField] private Vector2 _offsetXZ;
    [SerializeField] private Vector2 _terrainSize;
    [SerializeField] private Vector2 _targetTextureSize;
    [SerializeField] private Texture2D _undiscoverdMap;
    [SerializeField] private Texture2D _discoveredMap;

    private int _texWidth;
    private int _texHeight;
    private Vector2 _minimapSize;
    private Vector2 _relationMultiplier;
    private Vector2 _realRelationMultiplier;

    private RenderTexture _renderTex;
    private RectTransform _undiscoveredTransform;
    private Material _undiscoveredMaterial;
    private Texture2D _undiscoveredCopyTex;

    private RectTransform _discoveredTransform;
    private Material _discoveredMaterial;

    private Transform _player;
    private RectTransform _playerMarker;

    private float _zoom = 1.0f;

    void Start()
    {
        SingleTons.MinimapManager = this;

        _texWidth = _undiscoverdMap.width;
        _texHeight = _undiscoverdMap.height;
        _minimapSize = Camera.main.transform.GetChild(0).GetChild(6).GetComponent<RectTransform>().sizeDelta;

        _undiscoveredTransform = Camera.main.transform.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        _undiscoveredMaterial = Camera.main.transform.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().material;
        _undiscoveredCopyTex = new Texture2D(_texWidth, _texHeight, TextureFormat.RGBA32, false);
        _undiscoveredCopyTex.SetPixels(_undiscoverdMap.GetPixels());
        _undiscoveredCopyTex.Apply();
        _undiscoveredTransform.gameObject.SetActive(false);

        //_renderTex = new RenderTexture(_texWidth, _texHeight, 32, RenderTextureFormat.ARGB32);
        //_undiscoveredMaterial.SetTexture("_MainTex", _undiscoveredCopyTex);
        //_undiscoveredMaterial.SetFloat("_MainTexWidth", _texWidth);
        //_undiscoveredMaterial.SetFloat("_MainTexHeight", _texHeight);
        //_undiscoveredMaterial.SetFloat("_Radius", _radius);
        //_undiscoveredMaterial.SetFloat("_RadiusSquared", _radius * _radius);

        _discoveredTransform = Camera.main.transform.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        _discoveredMaterial = Camera.main.transform.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetComponent<Image>().material;
        _discoveredMaterial.mainTexture = _discoveredMap;

        _relationMultiplier = new Vector2(_targetTextureSize.x / _terrainSize.x, _targetTextureSize.y / _terrainSize.y);
        _realRelationMultiplier = new Vector2(_texWidth / _terrainSize.x, _texHeight / _terrainSize.y);

        _player = SingleTons.GameController.Player.transform;
        _playerMarker = Camera.main.transform.GetChild(0).GetChild(6).GetChild(0).GetChild(1).GetComponent<RectTransform>();
    }

    void Update()
    {
        _playerMarker.localRotation = Quaternion.Euler(0, 0, -Camera.main.transform.eulerAngles.y);

        float relativeToWorldX = (_player.position.x) * _relationMultiplier.x;
        float relativeToWorldZ = (_player.position.z) * _relationMultiplier.y;
        _discoveredTransform.localPosition = new Vector2(((-relativeToWorldX - _offsetXZ.x) * _zoom - _minimapSize.x * 0.5f), ((-relativeToWorldZ - _offsetXZ.y) * _zoom - _minimapSize.y * 0.5f));
        float realRelativeX = _player.position.x * _realRelationMultiplier.x;
        float realRelativeZ = _player.position.z * _realRelationMultiplier.y;
        //_discoveredTransform.localPosition = new Vector2(((-realRelativeX - _offsetXZ.x) * _zoom - _minimapSize.x * 0.5f), ((-realRelativeZ - _offsetXZ.y) * _zoom - _minimapSize.y * 0.5f));
        _discoveredTransform.localScale = new Vector3(_zoom, _zoom, 1);

        //_undiscoveredMaterial.SetFloat("_PlayerPositionX", relativeToWorldX);
        //_undiscoveredMaterial.SetFloat("_PlayerPositionY", relativeToWorldZ);

        //_renderTex.Release();
        //Graphics.Blit(_undiscoveredCopyTex, _renderTex, _undiscoveredMaterial);
        //Graphics.CopyTexture(_renderTex, _undiscoveredCopyTex);
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
}
