using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A range of floats, min cannot be greater than max
/// </summary>
[System.Serializable]
public struct FloatRange
{
    [SerializeField]
    private float min;
    public float Min { get { return min; } set { min = value; Validate(); } }
    
    [SerializeField]
    private float max;
    public float Max { get { return max; } set { max = value; Validate(); } }

    public float Difference
    {
        get { return max - min; }
    }

    public float Lerp(float t) 
    {
        return Mathf.Lerp(min, max, t);
    }

    public float Middle => max - Difference;

    public FloatRange(int _min, int _max)
    {
        min = _min;
        max = _max;
        Validate();
    }

    public void Validate()
    {
        if (min > max)
        {
            float temp = min;
            min = max;
            max = temp;
        }

        if (min == max)
        {
            min--;
        }
    }

    /// <returns>whether a value is between min and max (inclusive)</returns>
    public bool Contains(float test)
    {
        return test >= min && test <= max;
    }

    /// <returns>a random value between min and max (inclusive)</returns>
    public float RandomValue()
    {
        return Random.Range(min, max);
    }

    public float Clamp(float value) 
    {
        return Mathf.Clamp(value, min, max);
    }
}

/// <summary>
/// A range of Ints, min cannot be greater or equal to than max
/// </summary>
[System.Serializable]
public struct IntRange
{
    [SerializeField]
    private int min;
    public int Min { get { return min; } set { min = value; Validate(); } }

    [SerializeField]
    private int max;
    public int Max { get { return max; } set { max = value; Validate(); } }

    public int Difference
    {
        get { return max - min; }
    }

    public IntRange(int _min, int _max)
    {
        min = _min;
        max = _max;
        Validate();
    }

    private void Validate() 
    {
        if (min > max)
        {
            int temp = min;
            min = max;
            max = temp;
        }
        
        if (min == max)
        {
            min--;
        }
    }

    /// <returns>whether a value is between min and max (inclusive)</returns>
    public bool Contains(float test)
    {
        return test >= min && test <= max;
    }

    /// <returns>a random value between min and max (inclusive)</returns>
    public float RandomValue()
    {
        return Random.Range(min, max+1);
    }
}
