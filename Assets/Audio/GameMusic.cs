using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class GameMusic
{
    public enum MixerVar { BASE_VOL, LAYER1_VOL, LAYER2_VOL, LAYER3_VOL }
    private string[] variableStrings = new string[4] { "Music_Base_Vol", "Music_1_Vol", "Music_2_Vol", "Music_3_Vol" };
    private float[] targetValues;
    private float[] currentValues;
    private string VarStr(MixerVar var) { return variableStrings[(int)var]; }

    [SerializeField]
    private AudioMixer mixer;
    public float transitionRate = 2.5f;
    public int TrackCount => variableStrings.Length;

    GameMusic() 
    {
        targetValues = new float[variableStrings.Length];
        currentValues = new float[variableStrings.Length];
    }

    public void Tick()
    {
        for (int i = 0; i < variableStrings.Length; ++i)
        {
            if (!Mathf.Approximately(currentValues[i], targetValues[i]))
            {
                currentValues[i] = Mathf.Lerp(currentValues[i], targetValues[i], transitionRate * Time.deltaTime);
            }
            else
            {
                currentValues[i] = targetValues[i];
            }
            mixer.SetFloat(VarStr((MixerVar)i), currentValues[i]);
        }
    }

    public void SetMixerVariable(MixerVar variable, float value, bool instant = false)
    {
        value = Math.Clamp(value - 80, -80, 20); //shift the db values so 0 is -80db(muted), 100 is 20db;
        targetValues[(int)variable] = value;

        if (instant)
        {
            currentValues[(int)variable] = value;
        }
    }
}
