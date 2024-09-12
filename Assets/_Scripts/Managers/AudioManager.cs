using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : PersistentSingleton<AudioManager>
{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip _removingItem;
    [SerializeField] private AudioClip _button;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayRemovingItem() => _PlaySound(_removingItem);

    public void PlayButton() => _PlaySound(_button);

    private void _PlaySound(AudioClip sfx)
    {
        _audioSource.clip = sfx;
        _audioSource.Play();
    }
}
