/*
Load
Establishes permanent objects when 
the app is initially launched.

Copyright 2024 John M. Quick.
*/

using System.Collections;
using UnityEngine;

public class Load : MonoBehaviour {

    //delay for showing logo, in seconds
    public float delay;

    //app settings
    public int screenW;
    public int screenH;
    public bool isFullscreen;

    //fade transition script, assigned in Inspector
    public FadeUI fade;

    //awake
    private void Awake() {

        /*
        We want to enforce the screen resolution 
        on startup to prevent problems with the 
        window in the executable build.
        */

        //force default screen resolution
        Screen.SetResolution(screenW, screenH, isFullscreen);

        /*
        The state manager singleton initializes 
        itself by design when called upon for 
        the first time.
        */

        //init state manager
        if (StateManager.Instance) {
        };
    }

    //start
    private void Start() {

        //start loading process
        StartCoroutine(LoadApp());
    }

    //load the app
    private IEnumerator LoadApp() {

        /*
        A coroutine is used to allow the execution 
        of the function over multiple frames.
        
        This allows us to easily use Unity's built-in 
        function to delay for transition effects, such 
        as the time to display a logo, fade in and out, 
        or play audio.
        */

        //reveal scene
        fade.Reveal();

        //delay for transition
        yield return new WaitForSeconds(fade.duration);

        //delay for logo appearance
        yield return new WaitForSeconds(delay);

        //conceal scene
        fade.Conceal();

        //switch scene
        StartCoroutine(StateManager.Instance.SwitchSceneTo("Menu", fade.duration));
    }
}