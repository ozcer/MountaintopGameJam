using UnityEngine;

public class SingleTimeSound : MonoBehaviour {
  private AudioSource _audio;
  private bool _audioStarted = false;

  private void Awake() {
    _audio = GetComponent<AudioSource>();
    _audio.pitch = Random.Range(0.75f, 1.25f);
  }

  public void ScaleVolume(float scale) {
    _audio.volume *= scale;
  }

  public void ScalePitch(float scale) {
    _audio.pitch *= scale;
  }

  public void RandomizePitch(float minPitch, float maxPitch) {
    _audio.pitch = Random.Range(minPitch, maxPitch);
  }
  
  public void LoadClipAndPlay(AudioClip clip) {
    _audio.clip = clip;
    _audio.Play();

    _audioStarted = true;
  }

  private void Update() {
    if (_audioStarted && _audio.isPlaying == false) {
      Destroy(gameObject);
    }
  }
}