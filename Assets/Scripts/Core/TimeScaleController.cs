using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    private static HashSet<ITimeScaleMultiplyer> _multiplyers = new HashSet<ITimeScaleMultiplyer>();

    private void Update()
    {
        var timeScale = 1f;

        foreach(var m in _multiplyers)
        {
            timeScale *= m.TimeScale;
        }

        Time.timeScale = timeScale;
    }

    public static void Add(ITimeScaleMultiplyer mult)
    {
        _multiplyers.Add(mult);
    }

    public static void Remove(ITimeScaleMultiplyer mult)
    {
        _multiplyers.Remove(mult);
    }

    public interface ITimeScaleMultiplyer
    {
        [HideInInspector]
        public float TimeScale { get; }
    }
}
