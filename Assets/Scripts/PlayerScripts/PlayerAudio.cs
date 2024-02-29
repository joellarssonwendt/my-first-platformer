using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // Cache
    private static AudioSource classAudioSource;
    private AudioSource audioSource;

    // Variables
    [SerializeField] private AudioClip[] pickUpSounds, footStepSounds, swordSwingSounds;
    [SerializeField] private AudioClip jumpSound, dodgeSound, hurtSound, smallHeartSound, transformSound, transformExitSound, unlockSound, swordItemSound, winSound;

    // Methods
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        classAudioSource = GetComponentInParent<AudioSource>();
    }

    public static void PlaySound(AudioClip sound, float volume)
    {
        classAudioSource.PlayOneShot(sound, volume);
    }

    public static void PlaySound(AudioClip[] sounds, float volume)
    {
        classAudioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)], volume);
    }

    public void JumpSound()
    {
        audioSource.PlayOneShot(jumpSound, 1.0f);
    }

    public void DodgeSound()
    {
        audioSource.PlayOneShot(dodgeSound, 2.0f);
    }

    public void PickUpSounds()
    {
        audioSource.PlayOneShot(pickUpSounds[Random.Range(0, pickUpSounds.Length)], 1.0f);
    }

    public void LifeOrbSound()
    {
        audioSource.PlayOneShot(smallHeartSound, 1.0f);
    }

    public void SwordItemSound()
    {
        audioSource.PlayOneShot(swordItemSound, 1.0f);
    }

    public void TransformSound()
    {
        audioSource.PlayOneShot(transformSound, 1.0f);
    }

    public void TransformExitSound()
    {
        audioSource.PlayOneShot(transformExitSound, 2.0f);
    }

    public void UnlockSound()
    {
        audioSource.PlayOneShot(unlockSound, 1.0f);
    }

    public void WinSound()
    {
        audioSource.PlayOneShot(winSound, 1.0f);
    }

    public void HurtSound()
    {
        audioSource.PlayOneShot(hurtSound, 1.0f);
    }

    private void FootStepSounds()
    {
        audioSource.PlayOneShot(footStepSounds[Random.Range(0, footStepSounds.Length)], 0.5f);
    }

    private void SwordSwingSounds()
    {
        audioSource.PlayOneShot(swordSwingSounds[Random.Range(0, swordSwingSounds.Length)], 1.0f);
    }
}
