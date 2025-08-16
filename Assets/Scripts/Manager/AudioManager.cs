using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // 싱글톤 인스턴스

    [Header("Mixer Settings")]
    public AudioMixer audioMixer; // 오디오 믹서 (BGM, SFX 볼륨 제어)

    [Header("Background Music")]
    public AudioSource bgmSource; // BGM 재생용 AudioSource
    public AudioClip[] bgmClips;  // 재생 가능한 BGM 클립 배열

    [Header("Sound Effects (Auto Register)")]
    public AudioSource sfxSource; // SFX 재생용 AudioSource
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>(); // SFX 이름-클립 매핑 딕셔너리

    private AudioSource loopSource; // 루프용 AudioSource

    void Awake()
    {
        // 싱글톤 패턴 처리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            LoadAllSFX(); // 효과음 자동 등록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Resources/Audio/SFX 폴더 내 모든 오디오 클립을 자동 등록
    void LoadAllSFX()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/SFX");
        foreach (AudioClip clip in clips)
        {
            if (!sfxDict.ContainsKey(clip.name))
            {
                sfxDict.Add(clip.name, clip);
                Debug.Log($"SFX 로드 완료: {clip.name}");
            }
        }
    }

    // 인덱스로 BGM 재생
    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // 이름으로 SFX 재생
    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' 을(를) 찾을 수 없습니다.");
        }
    }

    // 피치와 볼륨 설정하여 SFX 재생
    public void PlaySFX(string name, float volume, float pitch)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
            sfxSource.pitch = 1f; // 재생 후 피치 초기화
        }
    }

    // 위치 기반으로 SFX 재생 (3D 공간)
    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    // 루프 사운드 시작 (지속 재생 효과음)
    public void PlaySFXLoop(string name)
    {
        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.loop = true;
            loopSource.playOnAwake = false;
        }

        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            loopSource.clip = clip;
            loopSource.Play();
        }
    }

    // 루프 사운드 정지
    public void StopSFXLoop()
    {
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }

    // BGM 볼륨 설정 (0.0 ~ 1.0 범위, dB 변환)
    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    // SFX 볼륨 설정 (0.0 ~ 1.0 범위, dB 변환)
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }
}
