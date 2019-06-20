using UnityEngine;

public class ArtifactDummy : MonoBehaviour
{
    private ArtifactDummy _peer;
    private PlayerRadar _radar;
    private int _maxArtifactIndex;
    private int _minArtifactIndex;
    private bool _hasChangedTarget;
    private bool _isCloser;

    void Start()
    {
        ArtifactDummy[] dummies = transform.parent.GetComponentsInChildren<ArtifactDummy>();
        for (int i = 0; i < dummies.Length; i++)
        {
            if (dummies[i].gameObject == gameObject) continue;

            if (gameObject.name.Substring(0, 5) == dummies[i].gameObject.name.Substring(0, 5))
            {
                _peer = dummies[i];
                break;
            }
        }
        CheckClosest(_peer.gameObject);
        _radar = SingleTons.GameController.Player.GetComponentInChildren<PlayerRadar>();
        Transform[] children = transform.parent.GetComponentsInChildren<Transform>();
        _minArtifactIndex = 999;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag.Contains("Target"))
            {
                int index = 0;
                int.TryParse(children[i].tag.Substring(6), out index);
                if (_maxArtifactIndex <= index)
                    _maxArtifactIndex = index;
                if (_minArtifactIndex >= index)
                    _minArtifactIndex = index;
            }
        }

        SingleTons.SoundWaveManager.onFishScanEvent += CheckClosest;
    }

    public void CheckClosest(GameObject pGameObject)
    {
        if (SingleTons.QuestManager.GetCurrentTarget() == null) return;
        if (Vector3.Distance(transform.position, SingleTons.QuestManager.GetCurrentTarget().transform.position) >
            Vector3.Distance(_peer.transform.position, SingleTons.QuestManager.GetCurrentTarget().transform.position))
        {
            _isCloser = false;
        }
        else
        {
            _isCloser = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.tag != "Player") return;
        if (_radar.GetTarget() == gameObject)
        {
            if (_isCloser)
                _radar.SetTarget(SingleTons.QuestManager.GetCurrentTarget());
            else
                _radar.SetTarget(_peer.gameObject);

            _hasChangedTarget = true;
        }
        else if (_radar.GetTarget() != _peer && _peer._isCloser)
        {
            _radar.SetTarget(_peer.gameObject);
            _hasChangedTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger || other.tag != "Player") return;
        if (_radar.GetTarget() != gameObject && !_hasChangedTarget)
        {
            if (_maxArtifactIndex >= SingleTons.QuestManager.GetCurrentTargetIndex() && _minArtifactIndex <= SingleTons.QuestManager.GetCurrentTargetIndex())
            {
                _radar.SetTarget(gameObject);
            }
        }

        _hasChangedTarget = false;
    }
}
