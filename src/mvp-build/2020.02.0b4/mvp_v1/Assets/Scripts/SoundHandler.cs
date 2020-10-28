using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioSource audioSource;
    private Dictionary<SoundNames, AudioClip> sounds;

#pragma warning disable 649 //disable the "Field x is never assigned to" warning which is a roslyn compaitibility issue 
    [SerializeField] AudioClip arrow1;
    [SerializeField] AudioClip arrow2;
    [SerializeField] AudioClip sword;
    [SerializeField] AudioClip magic1;
    [SerializeField] AudioClip humanScream;
    [SerializeField] AudioClip orcScream;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        FillSoundDictionary();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FillSoundDictionary()
    {
        sounds = new Dictionary<SoundNames, AudioClip>();
        sounds.Add(SoundNames.arrow1, arrow1);
        sounds.Add(SoundNames.arrow2, arrow2);
        sounds.Add(SoundNames.sword, sword);
        sounds.Add(SoundNames.magic1, magic1);
        sounds.Add(SoundNames.humanScream, humanScream);
        sounds.Add(SoundNames.orcScream, orcScream);
    }

    internal void PlaySound(SoundNames sound)
    {
        var soundMatch = sounds.Where(a => a.Key == sound).FirstOrDefault();
        
        if (soundMatch.Value != null)
        {
            audioSource.PlayOneShot(soundMatch.Value);
        }
    }
}

public enum SoundNames
{
    arrow1,arrow2,sword,magic1,humanScream,orcScream
}
