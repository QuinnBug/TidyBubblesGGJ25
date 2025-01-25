using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField]
    GameAudio gameAudio;

    int musicLevel = 0;

    void SetMusicLayer(int layer, bool value) 
    {
        gameAudio.SetMixerVariable((GameAudio.MixerVar)layer, value ? 80 : 0);
    }

    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            musicLevel++;
        }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) 
        {
            musicLevel--;
        }
        UpdateMusicLevel();

        gameAudio.Tick();
    }

    void UpdateMusicLevel() 
    {
        musicLevel = Mathf.Clamp(musicLevel, -1, 2);

        for (int i = 0; i < 3; i++)
        {
            SetMusicLayer(i, musicLevel >= i);
        }
    }
}
