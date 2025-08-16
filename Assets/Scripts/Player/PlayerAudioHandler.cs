using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    private const string FOOTSTEP_CLIP = "PlayerFootStep";

    public void Init()
    {
        audioManager = AudioManager.Instance;
    }

    public void PlayStepSound()
    {
        int num = Random.Range(1, 3);
        string clipName = FOOTSTEP_CLIP + num.ToString();
        audioManager.PlaySFX(clipName);
    }
}
