/*
StateManager
Manages the application state 
and scene switching using a 
singleton instance.

Copyright 2024 John M. Quick.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {

    //singleton instance
    private static StateManager _Instance;

    //singleton accessor
    //access StateManager.Instance from other classes
    public static StateManager Instance {

        /*
        When the state manager is called upon 
        for the first time in our code, it 
        creates itself through the get accessor.

        There can only be one instance of a singleton, 
        so our script ensures that only one ever gets 
        created. Once it is created, subsequent calls 
        will always refer to the singleton instance.
        */

        //create instance via getter
        get {

            //check for existing instance
            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject StateManagerObj = new GameObject();
                StateManagerObj.name = "StateManager";

                //create instance
                _Instance = StateManagerObj.AddComponent<StateManager>();
            }

            //return instance
            return _Instance;
        }
    }

    //awake
    void Awake() {

        //prevent this script from being destroyed when application switches scenes
        DontDestroyOnLoad(this);
    }

    //switch scene by name
    public IEnumerator SwitchSceneTo(string theScene, float theDelay = 0.0f) {

        /*
        A coroutine is used to allow the execution 
        of the function over multiple frames.

        This allows us to easily use Unity's built-in 
        function to delay for transition effects, such 
        as the time to display a logo, fade in and out, 
        or play audio.
        */

        //delay for transition
        yield return new WaitForSeconds(theDelay);

        //load scene
        SceneManager.LoadScene(theScene);
    }
}