using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  [SerializeField] private GameObject _singleTimeSound;
  [SerializeField] private List<SoundAudioClip> _soundAudioClips;
  private float sfxVolume;

  private static SoundManager _instance;
  public static SoundManager Instance {
    get {
      return _instance;
    }
  }

  private void Awake() {
    if (_instance != null && _instance != this) {
      Destroy(this.gameObject);
    } else {
      _instance = this;
    }
  }

  public void PlaySound(Sound sound, float volumeScaling = 1.0f, float pitchScaling = 1.0f, float minPitch = 0.75f, float maxPitch = 1.25f) {
    AudioClip clip = GetClip(sound);
    sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 10f) / 10f;
    if (clip != null) {
      SingleTimeSound stSound = Instantiate(_singleTimeSound, transform.position, Quaternion.identity).GetComponent<SingleTimeSound>();

      stSound.ScaleVolume(volumeScaling * sfxVolume);
      stSound.ScalePitch(pitchScaling);
      stSound.RandomizePitch(minPitch, maxPitch);

      stSound.LoadClipAndPlay(clip);
    }
  }

  private AudioClip GetClip(Sound sound) {
    foreach (SoundAudioClip s in _soundAudioClips) {
      if (sound == s.sound) {
        return s.clip;
      }
    }
    
    return null;
  }  
}