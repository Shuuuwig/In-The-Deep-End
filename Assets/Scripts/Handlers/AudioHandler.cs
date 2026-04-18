using UnityEngine;
using UnityEngine.EventSystems; // Add this for EventSystem

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance { get; private set; }

    [SerializeField] private AudioClip _UISelectedSound;
    [SerializeField] private AudioSource _audioSource;

    private GameObject _lastSelectedObject; // Track the previous selection

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        if (_audioSource == null)
        {
            _audioSource = GetComponentInChildren<AudioSource>();
        }
    }

    private void Update()
    {
        // 1. Check if the EventSystem exists and something is selected
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            // 2. If the selection has changed since the last frame
            if (EventSystem.current.currentSelectedGameObject != _lastSelectedObject)
            {
                // 3. Play the sound (Use OneShot so it doesn't cut off music)
                PlayUISelectionSound();

                // 4. Update the tracker
                _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    private void PlayUISelectionSound()
    {
        if (_UISelectedSound != null)
        {
            _audioSource.PlayOneShot(_UISelectedSound);
        }
    }

    // Inside AudioHandler.cs
    public void PlayMusic(AudioClip audioClip, bool loop = false)
    {
        if (audioClip == null) return;

        if (_audioSource.clip == audioClip && _audioSource.isPlaying)
        {
            return;
        }

        _audioSource.clip = audioClip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip == null) return;

        _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void PlaySoundOneShot(AudioClip audioClip)
    {
        if (audioClip == null) return;
        _audioSource.PlayOneShot(audioClip);
    }
}