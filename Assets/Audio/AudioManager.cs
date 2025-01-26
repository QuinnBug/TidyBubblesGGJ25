using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField]
    GameMusic gameAudio;
    [Space]
    public GameObject fxSourcePrefab;
    public FloatRange pitchRange;
    public AudioClip[] fxClips;
    private Pool fxSources;
    [Space]
    public AudioClip[] voClips;
    public AudioSource voSource;
    public List<int> voClipOptions = new List<int>();
    
    int musicLevel = 0;

    private void Start()
    {
        for (int i = 0; i < voClips.Length; i++)
        {
            voClipOptions.Add(i);
        }
        fxSources = new Pool(transform, fxSourcePrefab, 5);
    }

    public void SetMusicLevel(int level) 
    {
        musicLevel = level;
    }

    void SetMusicLayer(int layer, bool value) 
    {
        gameAudio.SetMixerVariable((GameMusic.MixerVar)layer, value ? 80 : 0);
    }

    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            RandomVoiceLine();
        }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            PlayFXOneShot(Random.Range(0, fxClips.Length), true);
        }

        UpdateMusicLevel();

        gameAudio.Tick();
        fxSources.Update();
    }

    void UpdateMusicLevel() 
    {
        musicLevel = Mathf.Clamp(musicLevel, -1, gameAudio.TrackCount);

        for (int i = 0; i < gameAudio.TrackCount; ++i)
        {
            SetMusicLayer(i, musicLevel >= i);
        }
    }

    public void RandomVoiceLine() 
    {
        int idx = voClipOptions[Random.Range(0, voClipOptions.Count)];
        voClipOptions.Remove(idx);

        if (voClipOptions.Count == 0)
        {
            voClipOptions.Clear();
            for (int i = 0; i < voClips.Length; i++)
            {
                if(idx != i) voClipOptions.Add(i);
            }
        }

        PlayVoiceLine(idx);
    }

    public void PlayFXOneShot(int fxIndex, bool randomPitch) 
    {
        AudioSource source = fxSources.Fetch().GetComponent<AudioSource>();

        if (randomPitch)
        {
            source.pitch = pitchRange.RandomValue();
        }
        else
        {
            source.pitch = 1;
        }

        source.PlayOneShot(fxClips[fxIndex]);

        fxSources.DelayedDispose(source.gameObject, 1.0f);
    }

    void PlayVoiceLine(int voIndex) 
    {
        voSource.PlayOneShot(voClips[voIndex]);
    }
}
