using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionBehaviour : MonoBehaviour
{
    //lead-in completion
    bool leadInComplete;

    //button animation
    public CanvasGroup overlayObjects;
    public float overlayObjectsFadeDuration = 0.8f;
    public LeanTweenType overlayObjectsFadeEase = LeanTweenType.easeInOutSine;

    public Image curtain;
    public float curtainFadeDuration = 1f;
    public LeanTweenType curtainFadeEase = LeanTweenType.easeInOutSine;

    //play quack animation on mouse up
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        leadInComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (leadInComplete) //play charge animation
        {
            if (Input.GetMouseButtonDown(0)) anim.SetBool("Charging", true);
            if (Input.GetMouseButtonUp(0)) anim.SetBool("Charging", false);
        }
    }

    //buttons slide in (leantween) before LoadNextScene
    public void ShowOptionsMenu()
    {
        //show options menu
    }

    public void HideOptionsMenu()
    {
        //show options menu
    }

    public void InitFadeout()
    {
        LeanTween.alphaCanvas(overlayObjects, 0f, overlayObjectsFadeDuration).setEase(overlayObjectsFadeEase);
        LeanTween.alpha(curtain.rectTransform, 1f, curtainFadeDuration).setEase(curtainFadeEase).setOnComplete(LoadNextScene); //change to work on button press instead
    }

    public void setLeadInComplete()
    {
        leadInComplete = true;
        //Debug.Log("lead in done");
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
