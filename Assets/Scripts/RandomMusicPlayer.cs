using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    public AudioClip[] audioClips; // Tableau pour stocker les clips audio
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomMusic();
    }

    void PlayRandomMusic()
    {
        int randomIndex = Random.Range(0, audioClips.Length); // S�lectionne un index al�atoire
        audioSource.clip = audioClips[randomIndex]; // Assigne le clip audio
        audioSource.Play(); // Joue le clip
    }
}
