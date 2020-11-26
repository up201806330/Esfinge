using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource effects;
    AudioSource mainSource;
    AudioSource secondSource;

    // SFX
    public AudioClip memPickup;
    public AudioClip lastMemPickup;

    // Tracks
    [SerializeField]
    AudioClip[] music;


    public float maxVolume;
    [Space]
    public float fadeSpeed;
    bool mainIsPlaying;

    private class Source {
        public AudioSource source;
        public bool isFadingIn;
        public bool isFadingOut;

        public Source(AudioSource source) {
            this.source = source;
            isFadingIn = false;
            isFadingOut = false;
        }
    }
    Source MainSource, SecondSource;

    private void Start() {
        effects = GetComponents<AudioSource>()[0];
        mainSource = GetComponents<AudioSource>()[1];
        secondSource = GetComponents<AudioSource>()[2];
        MainSource = new Source(mainSource);
        SecondSource = new Source(secondSource);

        mainIsPlaying = false;
        mainSource.volume = 0;
        secondSource.volume = 0;

        changeMusic(0);
    }

    public void changeMusic(int index) {
        AudioClip newClip = music[index];
        if (mainIsPlaying) {
            secondSource.clip = newClip;
            StartCoroutine(fadeIn(SecondSource));
            StartCoroutine(fadeOut(MainSource));
            secondSource.Play();
            mainIsPlaying = false;
        }
        else {
            mainSource.clip = newClip;
            StartCoroutine(fadeIn(MainSource));
            StartCoroutine(fadeOut(SecondSource));
            mainSource.Play();
            mainIsPlaying = true;
        }
    }

    public void playSFX(int id) {
        if (id == 0) effects.PlayOneShot(memPickup);
        else if (id == 1) effects.PlayOneShot(lastMemPickup);
    }

    IEnumerator fadeIn(Source x) {
        x.isFadingIn = true;
        x.isFadingOut = false;

        AudioSource source = x.source;
        float newVolume = source.volume;
        while (source.volume < maxVolume && x.isFadingIn) {
            newVolume += fadeSpeed;
            source.volume = newVolume;
            yield return new WaitForSeconds(0.1f);
        }

    }

    IEnumerator fadeOut(Source x) {
        x.isFadingIn = false;
        x.isFadingOut = true;

        AudioSource source = x.source;
        float newVolume = source.volume;
        while (source.volume >= 0 && x.isFadingOut) {
            newVolume -= fadeSpeed;
            if (newVolume < 0) newVolume = 0;
            source.volume = newVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }
}


