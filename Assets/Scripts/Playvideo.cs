using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]

public class Playvideo : MonoBehaviour {

    public VideoPlayer videoPlayer;
   
    void Start () {
        videoPlayer.loopPointReached += EndReached;
	}
	
	void EndReached (VideoPlayer vp) {
        Debug.Log("End Reached");
        SceneManager.LoadScene("EnvOne");
    }

}
