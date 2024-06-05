using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private AlarmTrigger _alarmTrigger;
    [SerializeField] private List<AudioSource> _audioSources;
    [SerializeField][Range(0, 1)] private float _maxVolumeValue = 1;
    [SerializeField][Range(0, 1)] private float _minVolumeValue = 0;
    [SerializeField] private float _secondsToChangeVolume;

    private float _deltaVolumeValue;
    private List<Coroutine> _currentCoroutines = new();

    private void Awake()
    {
        _deltaVolumeValue = (_maxVolumeValue - _minVolumeValue) / _secondsToChangeVolume;
    }

    private void OnValidate()
    {
        if (_maxVolumeValue < _minVolumeValue)
        {
            _maxVolumeValue = 1;
            _minVolumeValue = 0;
        }
    }

    private void OnEnable()
    {
        _alarmTrigger.Enabled += StartPlaySound;
        _alarmTrigger.Disabled += StopPlaySound;
    }

    private void OnDisable()
    {
        _alarmTrigger.Enabled -= StartPlaySound;
        _alarmTrigger.Disabled -= StopPlaySound;
    }

    private void StartPlaySound()
    {
        StopRunningCoroutines();

        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.Play();

            _currentCoroutines.Add(StartCoroutine(SetVolumeValueSmootly(audioSource, _minVolumeValue, _maxVolumeValue)));
        }
    }

    private void StopPlaySound()
    {
        StopRunningCoroutines();

        foreach (AudioSource audioSource in _audioSources)
        {
            _currentCoroutines.Add(StartCoroutine(SetVolumeValueSmootly(audioSource, _maxVolumeValue, _minVolumeValue)));
        }
    }

    private void StopRunningCoroutines()
    {
        if (_currentCoroutines.Count != 0)
            foreach (Coroutine coroutine in _currentCoroutines)
                StopCoroutine(coroutine);
    }

    private IEnumerator SetVolumeValueSmootly(AudioSource audioSource, float from, float to)
    {
        var fixedDeltaTime = new WaitForFixedUpdate();

        while (audioSource.volume != to)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, to, _deltaVolumeValue * Time.fixedDeltaTime);
            yield return fixedDeltaTime;
        }

        if (from > to)
            audioSource.Stop();

        _currentCoroutines.RemoveAt(0); 
    }
}
