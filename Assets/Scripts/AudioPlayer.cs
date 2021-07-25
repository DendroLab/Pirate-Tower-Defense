using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    private static AudioPlayer _instance = null;

    public static AudioPlayer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioPlayer>();
            }

            return _instance;
        }
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _audioClips;

    public void PlaySFX(string name)
    {
        AudioClip sfx = _audioClips.Find(s => s.name == name);

        if (sfx == null)
        {
            return;
        }

        _audioSource.PlayOneShot(sfx);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
