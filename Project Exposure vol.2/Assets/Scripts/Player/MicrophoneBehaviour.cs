using UnityEngine;

public class MicrophoneBehaviour : MonoBehaviour
{
    private MeshCollider _collider;
    private SoundWaveManager _soundWaveManager;
    private Transform _playerTransform;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    void Start()
    {
        _collider = GetComponent<MeshCollider>();
        _soundWaveManager = SingleTons.SoundWaveManager;
        _playerTransform = SingleTons.GameController.Player.transform;
        _playerMovementBehaviour = _playerTransform.GetComponent<PlayerMovementBehaviour>();

        Physics.IgnoreLayerCollision(this.gameObject.layer, 9, true);
    }

    void Update()
    {
        if (_playerMovementBehaviour.GetIsFollowing())
        {
            transform.parent.rotation = _playerTransform.rotation * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.parent.rotation = Camera.main.transform.parent.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;

            AudioSource oSound = other.GetComponent<AudioSource>();
            oSound.maxDistance = _collider.bounds.extents.z * 1.5f;

            SingleTons.SoundWaveManager.GetListeningToAll.Add(other.transform.gameObject);

            foreach (string key in SingleTons.CollectionsManager.collectedAudioSources.Keys)
            {
                if (other.transform.name == key)
                {
                    SingleTons.SoundWaveManager.GetListeningToCollected.Add(other.transform.gameObject);
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;

            other.GetComponent<AudioSource>().maxDistance = SingleTons.CollectionsManager.GetMaxDistance;

            if (other.tag == "Collectable")
                _soundWaveManager.HideProgress(other.transform.gameObject);

            SingleTons.SoundWaveManager.GetListeningToAll.Remove(other.transform.gameObject);

            for (int i = 0; i < SingleTons.SoundWaveManager.GetListeningToCollected.Count; i++)
                if (other.transform.gameObject == SingleTons.SoundWaveManager.GetListeningToCollected[i])
                    SingleTons.SoundWaveManager.GetListeningToCollected.RemoveAt(i);
        }
    }
}
