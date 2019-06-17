using UnityEngine;

public class ScannerBehaviour : MonoBehaviour
{
    private SoundWaveManager _soundWaveManager;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    void Start()
    {
        _soundWaveManager = SingleTons.SoundWaveManager;
        _playerMovementBehaviour = SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.gameObject.layer == 10)
            _soundWaveManager.HideProgress(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;

            if (other.tag == "Collectable")
            {
                bool isClickingTarget = false;
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    RaycastHit[] hit;
                    hit = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30.0f, ~(1 << 8));
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i].collider.isTrigger) continue;

                        Transform[] trs = hit[i].transform.GetComponentsInChildren<Transform>();
                        for (int j = 0; j < trs.Length; j++)
                        {
                            if (trs[j].gameObject == other.gameObject)
                            {
                                isClickingTarget = true;
                                if (Input.GetKeyDown(KeyCode.Mouse0))
                                    _playerMovementBehaviour.StartFollowingGameObject(other.gameObject);
                                break;
                            }
                        }
                    }
                }

                if (isClickingTarget)
                {
                    if (_playerMovementBehaviour.CheckIfFollowingGameObject(other.gameObject) &&
                        !SingleTons.CollectionsManager.IsCollected(other.gameObject.name))
                    {
                        for (int i = 0; i < SingleTons.SoundWaveManager.GetListeningToCollected.Count; i++)
                            if (SingleTons.SoundWaveManager.GetListeningToCollected[i] == other.gameObject) return;

                        _soundWaveManager.ScanCreature(other.gameObject);
                        _soundWaveManager.ShowProgress(other.gameObject);
                    }
                }
                else
                {
                    _soundWaveManager.HideProgress(other.gameObject);
                }
            }
            else if (other.tag.Substring(0, 6) == "Target")
            {
                if (SingleTons.CollectionsManager.HasTargetBeenScanned(other.tag)) return;
                _soundWaveManager.ScanTarget(other.gameObject);
                _soundWaveManager.ShowProgress(other.gameObject);
            }
        }
    }
}
