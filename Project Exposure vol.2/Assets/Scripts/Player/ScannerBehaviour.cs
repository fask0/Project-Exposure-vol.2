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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;
            _soundWaveManager.StopScanning(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;

            if (other.tag == "Collectable")
            {
                _soundWaveManager.ScanCreature(other.gameObject);
                _soundWaveManager.ShowProgress(other.gameObject);
            }
            else if (other.tag.Substring(0, 6) == "Target")
            {
                _soundWaveManager.ScanTarget(other.gameObject);
                _soundWaveManager.ShowProgress(other.gameObject);
            }
        }
    }
}
