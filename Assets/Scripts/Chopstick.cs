using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chopstick : MonoBehaviour
{
    public float spinDuration = 2f;
    public LeanTweenType spinEaseType = LeanTweenType.easeInOutSine;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360f, spinDuration).setLoopClamp().setEase(spinEaseType);
    }

    // On collision with player, go to next scene, 2d trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
