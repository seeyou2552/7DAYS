using UnityEngine;

public class VehicleAudioHandler : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip startEngineClip;
    [SerializeField] AudioClip idleEngineClip;
    [SerializeField] AudioClip stopEngineClip;

    public void Init()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayVehicleStart()
    {
        audioSource.PlayOneShot(startEngineClip);
        SetVehicleIdle(true);
    }

    public void PlayVehicleStop()
    {
        SetVehicleIdle(false);
        audioSource.PlayOneShot(stopEngineClip);
    }

    void SetVehicleIdle(bool isStart)
    {
        if (isStart)
        {
            audioSource.clip = idleEngineClip;
            audioSource.Play();

        }
        else
        {
            audioSource.Stop();
        }
    }

    public void SetPitch(bool isMove)
    {
        audioSource.pitch = isMove ? 2 : 1;
    }
}
