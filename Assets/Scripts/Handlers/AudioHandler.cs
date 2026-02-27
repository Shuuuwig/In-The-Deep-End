using UnityEngine;

public static class AudioHandler
{
    public static void PlaySound(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();      
    }
}
