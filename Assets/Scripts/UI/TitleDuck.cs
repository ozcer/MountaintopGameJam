using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleDuck : MonoBehaviour
{
    public float bobDuration = 1f;
    public float bobHeight = 10f;
    public LeanTweenType bobEase = LeanTweenType.easeInOutSine;
    
    public float finalZ = -2f;
    public float finalZDuration = 5f;
    public LeanTweenType finalZEase = LeanTweenType.easeInOutSine;

    public Image curtain;
    public float curtainFadeDuration = 1f;
    public LeanTweenType curtainFadeEase = LeanTweenType.easeInOutSine;
    
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveY(gameObject, transform.position.y + bobHeight, bobDuration).setLoopPingPong().setEase(bobEase);
        LeanTween.moveZ(gameObject, finalZ, finalZDuration).setEase(finalZEase);
    }

    // Update is called once per frame
    void Update()
    {
        // load next scene on left click up
        if (Input.GetMouseButtonUp(0))
        {
            LeanTween.alpha(curtain.rectTransform, 1f, curtainFadeDuration).setEase(curtainFadeEase).setOnComplete(LoadNextScene);
        }
    }
    
    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
