using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public int currentLevel = 0;

    public int numLevels;

    [HideInInspector]
    public Transform checkpoint;

    public bool next = false;

    public AudioClip[] levelMusic;

    public AudioSource music;
    public Slider musicSlider;
    public AudioSource fx;
    public Slider fxSlider;

    public Animator anim;
    public Image black;

    [HideInInspector]
    public bool nextLevel = false;

    bool destroy = false;

    public GameObject options;

    public void Start() {
        DontDestroyOnLoad(gameObject);

        if (levelMusic[currentLevel]) {
            music.clip = levelMusic[currentLevel];
            music.Play();
            StartCoroutine(FadeMusic(music, 0.5f, 0.5f));
        }
    }

    public void Update() {
        if (next) {
            next = false;
            nextLevel = true;
        }

        if (nextLevel) {
            nextLevel = false;
            StartCoroutine(FadeScene());
        }

        if (destroy) {
            Destroy(this.gameObject);
        }

    }

    public void StartButton() {
        nextLevel = true;
    }

    public void ResetLevel() {
        SceneManager.LoadScene(currentLevel);
    }

    public void NextLevel() {

        if (currentLevel >= numLevels - 1) {
            currentLevel = 0;
            destroy = true;
        } else {
            currentLevel++;
        }

        if (levelMusic[currentLevel]) {
            music.clip = levelMusic[currentLevel];
            music.Play();
        } else {
            music.Stop();
        }

        SceneManager.LoadScene(currentLevel);
    }

    public void PlaySound(AudioClip sound) {
        fx.PlayOneShot(sound);
    }

    public void ChangeMusicVolume() {
        music.volume = musicSlider.value;
    }

    public void ChangeFXVolume() {
        fx.volume = fxSlider.value;
    }

    public void ExitToMainMenu() {
        options.SetActive(false);
        if (currentLevel != 0) {
            currentLevel = numLevels - 1;
            nextLevel = true;
          
        }
    }

    IEnumerator FadeMusic(AudioSource audioSource, float duration, float targetVolume) {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    IEnumerator FadeScene() {
        black.canvas.sortingOrder = 1;
        anim.SetBool("fade", true);
        StartCoroutine(FadeMusic(music, 0.5f, 0));
        yield return new WaitUntil(() => black.color.a == 1);
        anim.SetBool("fade", false);
        NextLevel();
        StartCoroutine(FadeMusic(music, 0.5f, musicSlider.value));
        yield return new WaitUntil(() => black.color.a == 0);
        black.canvas.sortingOrder = -1;
    }
}
