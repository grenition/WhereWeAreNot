using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum PostProccesingType
{
    standart,
    bulletTime
}

[RequireComponent(typeof(Volume))]
public class GlobalPostProcessing : MonoBehaviour
{
    public static GlobalPostProcessing Instance { get; private set; }

    [Header("Base Volume")]
    [SerializeField] private VolumeProfile defaultProfile;
    [SerializeField] private VolumeProfile bulletTimeProfile;
    [SerializeField] private float transitionTime = 0.3f;

    [Header("Health Volume")]
    [SerializeField] private Volume healthVolume;

    private Volume volume;
    private Volume tempVolume;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        volume = GetComponent<Volume>();
        if (tempVolume == null)
        {
            tempVolume = gameObject.AddComponent<Volume>();
            SetTempVolumeWeight(0f);
        }
    }
    private void SetTempVolumeWeight(float weight)
    {
        tempVolume.weight = weight;
        tempVolume.enabled = weight != 0f;
    }
    private void SetNewProfile(VolumeProfile profile, float transitionTime)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToProfileCoroutine(profile, transitionTime));
    }
    private IEnumerator TransitionToProfileCoroutine(VolumeProfile profile, float transitionTime)
    {
        float startTime = Time.realtimeSinceStartup;
        tempVolume.profile = profile;
        while(Time.realtimeSinceStartup < startTime + transitionTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / transitionTime;
            SetTempVolumeWeight(t);
            volume.weight = 1 - t;
            yield return null;
        }
        volume.profile = profile;
        volume.weight = 1f;
        SetTempVolumeWeight(0f);
    }

    public static void SetProfile(PostProccesingType postProccesingType)
    {
        if (Instance == null)
            return;
        switch (postProccesingType)
        {
            case PostProccesingType.standart:
                Instance.SetNewProfile(Instance.defaultProfile, Instance.transitionTime);
                break;
            case PostProccesingType.bulletTime:
                Instance.SetNewProfile(Instance.bulletTimeProfile, Instance.transitionTime);
                break;
        }
    }

    public static void SetHealthVolumeWeight(float weight)
    {
        if (Instance == null || Instance.healthVolume == null)
            return;
        Instance.healthVolume.weight = weight;
    }
}
