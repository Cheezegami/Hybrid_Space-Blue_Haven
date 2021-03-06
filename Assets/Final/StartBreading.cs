﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBreading : MonoBehaviour {

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;
    int drawDepth = -1000; 
    float alpha = 1.0f;
    int fadeDir = -1;
    public int currentScene = 0;
    public bool startSceneOne = false;

    public float startCoundown = 4;
    public float currentCoundown = 0;
    public Texture2D color;
    public Texture2D colorBreathing;
    public Animator breathing_anim;

    public bool breathingOn = false;
    public bool alpha0 = true;   

    void Awake(){
        DontDestroyOnLoad(this);       
    }

    private void Update(){

        if (Input.GetKeyDown("b")){
            if (SceneManager.GetActiveScene().name != "StartScene" && currentCoundown <= 0){
                breathingOn = !breathingOn;          
            }
        }
        if (Input.GetKeyDown("1") || startSceneOne){
            startSceneOne = false;
            currentScene = 1;
            StartCoroutine(Fade());
        }
        if (Input.GetKeyDown("2")){
            currentScene = 2;
            StartCoroutine(Fade());
        }

        if (Input.GetKeyDown("9")){
            if (MovementManager.instance)MovementManager.instance.Pause();
        }
        if (Input.GetKeyDown("8")){
            if(MovementManager.instance)MovementManager.instance.UnPause();
        }

        if (currentCoundown > 0){
            currentCoundown = currentCoundown - Time.deltaTime;
        }

        if (Sequence.instance != null){
            if(Sequence.instance.ProgressToNextScene)
                StartCoroutine(ProgressToNextScene());
        }

        if (breathingOn){
            if (currentScene != 3){
                currentScene = 3;
                StartCoroutine(Fade());
                if(MovementManager.instance)MovementManager.instance.Pause();
            }
        }
        if (breathingOn == false && SceneManager.GetSceneByBuildIndex(3).isLoaded){
            StartCoroutine(FadeBreath());
        }
        if (SceneManager.GetSceneByBuildIndex(2).isLoaded && currentCoundown <= 0)
        {
            GameObject rt = GameObject.Find("RightTarget");
            rt.GetComponent<Orb>().enabled = true;
            GameObject lt = GameObject.Find("LeftTarget");
            lt.GetComponent<Orb>().enabled = true;
        }

    }

    IEnumerator FadeBreath(){
        float fadeTime = BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        currentScene = 1;
        SceneManager.UnloadSceneAsync(3);
        StartTimer();
        StartCoroutine(StartAni());
        BeginFade(-1);
    }

    IEnumerator Fade(){
        if (currentCoundown <= 0){
            int c = currentScene;
            float fadeTime = BeginFade(1); 
            yield return new WaitForSeconds(fadeTime);
            //SceneManager.LoadScene(c);
            if (c == 3){
                SceneManager.LoadScene(c, LoadSceneMode.Additive);
                yield return new WaitForEndOfFrame();
                OnLevelWasLoaded(3);
            }
            else{
                SceneManager.LoadScene(c, LoadSceneMode.Single);
            }
        }
    }

    void StartTimer(){
        currentCoundown = startCoundown;
    }

    private void OnGUI(){
        if (currentCoundown > 0){
            if (SceneManager.GetSceneByBuildIndex(3).isLoaded){
                GUI.DrawTexture(new Rect(0, Screen.height - 10, Screen.width * (currentCoundown / startCoundown), 10), colorBreathing);
            }
            else{
                GUI.DrawTexture(new Rect(0, Screen.height - 10, Screen.width*(currentCoundown/startCoundown), 10), color);
            }
        }

        alpha += fadeDir * fadeSpeed * Time.deltaTime;  
        alpha = Mathf.Clamp01(alpha);
        if (alpha == 0){
            alpha0 = true;
        }
        else{
            alpha0 = false;
        }
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public float BeginFade(int direction){
        fadeDir = direction;
        return (fadeSpeed);
    }

    private void OnLevelWasLoaded(int level){
        BeginFade(-1);
        StartTimer();
        StartCoroutine(StartAni());
        if (level == 2){
            Debug.Log("dsa");
            GameObject rt = GameObject.Find("RightTarget");
            rt.GetComponent<Orb>().enabled = false;
            GameObject lt = GameObject.Find("LeftTarget");
            lt.GetComponent<Orb>().enabled = false;
        }
    }
    
    private IEnumerator ProgressToNextScene(){
        MovementManager.instance.DisableCurrentMovement();
        yield return new WaitForSeconds(5f);

        if (currentScene == 1){
            currentScene = 2;
            StartCoroutine(Fade());
        }
    }

    IEnumerator StartAni(){
        yield return new WaitForSeconds(startCoundown);
        if (SceneManager.GetSceneByBuildIndex(3).isLoaded){
            breathing_anim = GameObject.Find("breathing_anim").GetComponent<Animator>();
            breathing_anim.SetBool("start", true);
        }
        else{
            if(MovementManager.instance)MovementManager.instance.UnPause();
        }
    } 
}
/*
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBreading : MonoBehaviour {

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 1.5f;
    int drawDepth = -1000; 
    float alpha = 1f;
    int fadeDir = -1;
    public int curretScene = 0;

    public List<Animator> allAniInScene = new List<Animator>();
    public float startCoundown = 4;
    public float currentCoundown;
    public Texture2D color;

    public bool breathingOn = false;

    void Awake(){
        DontDestroyOnLoad(this);
    }

    private void Update(){

        if (Input.GetKeyDown("b")){
            if (SceneManager.GetActiveScene().name != "StartScene"){
                curretScene = 0;
                StartCoroutine(Fade());             
            }
        }

        if (Input.GetKeyDown("1")){
            curretScene = 2;
            StartCoroutine(Fade());
        }
        if (Input.GetKeyDown("2")){
            curretScene = 3;
            StartCoroutine(Fade());
        }

        if (Input.GetKeyDown("9")){
            MovementManager.instance.Pause();
        }
        if (Input.GetKeyDown("8")){
            MovementManager.instance.UnPause();
        }

        if (currentCoundown > 0){
            currentCoundown = currentCoundown - Time.deltaTime;
        }
    }

    IEnumerator Fade(){
        if (currentCoundown <= 0){
            int c = curretScene;
            float fadeTime = BeginFade(1); 
            yield return new WaitForSeconds(fadeTime);
            //SceneManager.LoadScene(c);
            if (c == 0){
                SceneManager.LoadScene(c, LoadSceneMode.Additive);
                yield return new WaitForEndOfFrame();
                OnLevelWasLoaded(0);
                //MovementManager.instance.Pause();
            }
            else{
                SceneManager.LoadScene(c, LoadSceneMode.Single);
            }
        }

    }

    void StartTimer(){
        currentCoundown = startCoundown;
    }

    private void OnGUI(){
        if (currentCoundown > 0){
            GUI.DrawTexture(new Rect(0, Screen.height - 10, Screen.width*(currentCoundown/startCoundown), 10), color);
        }

        alpha += fadeDir * fadeSpeed * Time.deltaTime;  
        alpha = Mathf.Clamp01(alpha);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

    }

    public float BeginFade(int direction){
        fadeDir = direction;
        return (fadeSpeed);
    }

    private void OnLevelWasLoaded(int level){
        BeginFade(-1);
        StartTimer();
        StartCoroutine(StartAni(false));
    }

    IEnumerator StartAni(bool b){
        object[] an = FindObjectsOfType(typeof(Animator));
        foreach (var item in an){
            if (item.ToString().Contains("RightHand") || item.ToString().Contains("Lefthand") || item.ToString().Contains("breathing")){
                allAniInScene.Add(item as Animator);
            }
        }
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < allAniInScene.Count; i++){
            allAniInScene[i].enabled = false;
        }

        yield return new WaitForSeconds(startCoundown);
        for (int i = 0; i < allAniInScene.Count; i++){
            allAniInScene[i].enabled = true;
        }
        allAniInScene.Clear();
  

        /*
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < an.Length; i++){
            Debug.Log(an.Length);
            Animator a = an[i] as Animator;
            Debug.Log(a);
            a.enabled = b;
        }
    }

}
}
}*/
