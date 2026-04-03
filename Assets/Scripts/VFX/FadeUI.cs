/*
FadeUI
Handles fade in and out transparency transitions.
Designed to manage transparency for UI Toolkit.

Copyright 2024 John M. Quick.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FadeUI : MonoBehaviour {

    //UI document, assigned in Inspector
    public UIDocument doc;

    //name of overlay object in UI document
    public string overlayName;

    //overlay object in UI document
    private VisualElement _overlay;

    //alpha transition settings
    public float alphaMin;      //minimum alpha (usually 0)
    public float alphaMax;      //maximum alpha (usually 1)
    public float alphaInitial;  //initial alpha
    public float duration;      //duration, in seconds

    //current alpha level
    private float _alphaCurrent;

    //awake
    private void Awake() {

        /*
        The UI Toolkit is managed in code via a UI Document.
        Every UI Document has a root, which can be accessed 
        in code. From there, a query system allows us to 
        access each individual UI element.
        */
        
        //retrieve UI document root
        VisualElement root = doc.rootVisualElement;

        //retrieve overlay
        _overlay = root.Q<VisualElement>(overlayName);

        //set initial alpha
        _overlay.style.opacity = alphaInitial;

        //update current alpha
        _alphaCurrent = alphaInitial;
    }

    //conceal (hide screen, fade in overlay)
    public void Conceal() {

        /*
        Conceal is used to hide the screen from 
        the user. As the alpha of the overlay 
        increases, the visibility of the user 
        decreases.
        */

        //update current alpha
        _alphaCurrent = (float)_overlay.style.opacity.value;

        //start transition
        StartCoroutine(ConcealRoutine());
    }

    //reveal (show screen, fade out overlay)
    public void Reveal() {

        /*
        Reveal is used to show the screen to the 
        user. As the alpha of the overlay 
        decreases, the visibility of the user 
        increases.
        */

        //update current alpha
        _alphaCurrent = _overlay.style.opacity.value;

        //start transition
        StartCoroutine(RevealRoutine());
    }

    //conceal routine (from current alpha to max)
    private IEnumerator ConcealRoutine() {

        /*
        A coroutine is used to allow the execution 
        of the function over multiple frames. As 
        long as the target alpha value has not been 
        met, the function continues to execute each 
        frame, while incrementing the alpha value. 
        The current alpha value is determined by the 
        percentage of the total duration of the 
        transition that has elapsed. Once the target 
        is reached, the value is set equal to the 
        target and the coroutine ends.
        */

        //while transition is not complete
        while (_alphaCurrent < alphaMax) {

            //update alpha
            //time passed over frame / total duration as percentage of target
            _alphaCurrent += (Time.deltaTime / duration) * alphaMax;

            //if alpha exceeds limit
            if (_alphaCurrent > alphaMax) {

                //set to limit
                _alphaCurrent = alphaMax;
            }

            //update alpha in scene
            UpdateAlpha();

            //pause coroutine
            yield return 0;
        }
    }

    //reveal routine (from current alpha to min)
    private IEnumerator RevealRoutine() {

        /*
        A coroutine is used to allow the execution 
        of the function over multiple frames. As 
        long as the target alpha value has not been 
        met, the function continues to execute each 
        frame, while incrementing the alpha value. 
        The current alpha value is determined by the 
        percentage of the total duration of the 
        transition that has elapsed. Once the target 
        is reached, the value is set equal to the 
        target and the coroutine ends.
        */

        //while transition is not complete
        while (_alphaCurrent > alphaMin) {

            //update alpha
            //time passed over frame / total duration as percentage of target
            _alphaCurrent -= (Time.deltaTime / duration) * (alphaMax - alphaMin);

            //if alpha exceeds limit
            if (_alphaCurrent < alphaMin) {

                //set to limit
                _alphaCurrent = alphaMin;
            }

            //update alpha in scene
            UpdateAlpha();

            //pause coroutine
            yield return 0;
        }
    }

    //update the visible alpha in the scene
    private void UpdateAlpha() {
        
        //set alpha
        _overlay.style.opacity = _alphaCurrent;
    }

    //show (make the overlay and its contents visible)
    public void Show() {

        /*
        Sometimes the fade transition can be used 
        on a parent object, in which case it is not 
        concealing what is behind it, but showing 
        what is contained within it. Mathematically, 
        this is the same as concealing. However, to 
        prevent confusion, an alternate method is 
        provided with an easier to understand name.
        */

        //conceal
        Conceal();
    }

    //show (make the overlay and its contents invisible)
    public void Hide() {

        /*
        Sometimes the fade transition can be used 
        on a parent object, in which case it is not 
        revealing what is behind it, but hiding 
        what is contained within it. Mathematically, 
        this is the same as revealing. However, to 
        prevent confusion, an alternate method is 
        provided with an easier to understand name.
        */

        //reveal
        Reveal();
    }
}