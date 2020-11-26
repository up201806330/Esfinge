using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;
using System;

public class MemoryPickups : MonoBehaviour
{
    SoundManager soundManager;
    int currentMusic = 0;

    public Image mainMenu;
    public Image endScreen;
    public Image painting;
    public Image triangle;

    public Image wasd;
    public Image mouse;
    public Image shift;

    public VisualEffect vfx;
    float intensity = 0;

    public CinemachineFreeLook cam;
    public GameObject endCam;
    public ThirdPersonMovement movementController;

    public GameObject firstObjects;
    public GameObject finalObjects;

    public GameObject[] memories;
    public GameObject playerCam;
    public Indicator indicator;
    public int timeout = 50;

    public int currentMem = 0;

    public Material playerMat;
    public Material eyesMat;
    public float blinkAmount = 1f;
    public float blinkInTime = 1f;
    public float blinkOutTime = 1f;

    public GameObject mem6;

    private void Start() {
        soundManager = GetComponent<SoundManager>();

        foreach (GameObject mem in memories) mem.SetActive(false);
        memories[0].SetActive(true);

        firstObjects.SetActive(false);
        finalObjects.SetActive(false);

        playerMat.SetFloat("_Dissolve", 1f);
        eyesMat.SetFloat("_Dissolve", 1f);

        wasd.CrossFadeAlpha(0f, 0.001f, false);
        mouse.CrossFadeAlpha(0f, 0.001f, false);
        shift.CrossFadeAlpha(0f, 0.001f, false);

        endScreen.CrossFadeAlpha(0f, 0.001f, false);
        painting.CrossFadeAlpha(0f, 0.001f, false);
        triangle.CrossFadeAlpha(0f, 0.001f, false);

        TextMeshProUGUI[] texts = triangle.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) StartCoroutine(FadeOutText(100f, text));
    }

    public void StartGame() { // Called by main menu
        // Switch music
        soundManager.changeMusic(++currentMusic);

        // Fade out main menu
        mainMenu.CrossFadeAlpha(0f, 2f, false);
        TextMeshProUGUI[] texts = mainMenu.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) StartCoroutine(FadeOutText(2f, text));

        // Fade player model in
        StartCoroutine(BlinkTo(playerMat, "_Dissolve", 0f, 30f));
        StartCoroutine(BlinkTo(eyesMat, "_Dissolve", 0f, 30f));

        // Fade in UI with controls
        StartCoroutine(FadeUIControls());

        // Start timeout to auto activate first memory
        StartCoroutine(FirstMemTimeout());
    }

    private IEnumerator FirstMemTimeout() {
        yield return new WaitForSeconds(timeout);
        if (currentMem == 0) Activate(++currentMem);
    }

    public void Activate(int memoryIndex) {
        if (memoryIndex == memories.Length - 1) {
            // Avoids disappearing of obelisk
            mem6.SetActive(false);

            // Plays reverbed flutes instead of normal sound
            soundManager.playSFX(1);
        }
        else {
            // Deactivates memory before it
            memories[currentMem - 1].SetActive(false);

            // Plays normal pickup sound
            soundManager.playSFX(0);
        }

        // Won't blink player if it's first memory
        if (memoryIndex != 1) StartCoroutine(BlinkRoutine());

        // Show obelisk
        if (memoryIndex == memories.Length - 2) {
            firstObjects.SetActive(true);
        }

        if (memoryIndex == memories.Length - 1) {
            // Fade and destroy UI indicator
            StartCoroutine(indicator.FadeTo(0f, 1.5f));
            Destroy(indicator, 1.5f);

            // Fade out sandstorm
            StartCoroutine(FadeToWhiteAndOutVFX());

            // Deactivate Camera input
            movementController.ToggleControls();
            movementController.animationStateMachine(false, false, false, false);

            // Activate Final objects (pyramid and stuff)
            StartCoroutine(WaitAndActivateFinalObjects());

            // Wait and switch to end camera after sand dispersed
            StartCoroutine(WaitAndSwitchToEndCam());

            // Wait until camera reaches final position, then fade in white and then painting
            //CinemachineDollyCart dolly = endCam.GetComponent<CinemachineDollyCart>();
            TextMeshProUGUI[] texts = triangle.GetComponentsInChildren<TextMeshProUGUI>();
            StartCoroutine(FadeWhiteAndPainting(texts));
        }
        else {
            // Increase sandstorm intensity
            intensity += 0.45f;
            StartCoroutine(FadeVFX(intensity, 1.5f));
        }

        memories[memoryIndex].SetActive(true);

        if (memoryIndex == 1) {
            StartCoroutine(SwitchCameras(memoryIndex));
            StartCoroutine(indicator.FadeTo(indicator.minAlpha, 1.5f));
            // Switch to track 1
        }
        // Switch music
        soundManager.changeMusic(++currentMusic);

        // UI indicator update
        indicator.retarget(memories[memoryIndex].transform);
    }

    private IEnumerator WaitAndActivateFinalObjects() {
        yield return new WaitForSeconds(2f);
        finalObjects.SetActive(true);
    }

    private IEnumerator SwitchCameras(int memoryIndex) {
        if ((memoryIndex < memories.Length && memoryIndex >= 0)) {
            playerCam.SetActive(false); memories[memoryIndex].GetComponentInChildren<CinemachineVirtualCamera>().enabled = false;
            yield return new WaitForSeconds(3);
            memories[memoryIndex].GetComponentInChildren<CinemachineVirtualCamera>().enabled = true; playerCam.SetActive(true);
        }
    }

    IEnumerator BlinkRoutine() {
        yield return StartCoroutine(BlinkInAndOut());
    }

    IEnumerator BlinkInAndOut() {
        StartCoroutine(BlinkTo(playerMat, "_Blink", blinkAmount, blinkInTime));
        yield return new WaitForSeconds(blinkInTime);
        StartCoroutine(BlinkTo(playerMat, "_Blink", 0, blinkOutTime));
        yield return new WaitForSeconds(blinkOutTime);
    }

    IEnumerator BlinkTo(Material mat, string param, float aValue, float aTime) {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            mat.SetFloat(param, Mathf.Lerp(mat.GetFloat(param), aValue, t));
            yield return null;
        }
    }

    IEnumerator FadeVFX(float aValue, float aTime) {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            vfx.SetFloat("Intensity", Mathf.Lerp(vfx.GetFloat("Intensity"), aValue, t));
            yield return null;
        }
    }

    IEnumerator FadeToWhiteAndOutVFX() {
        yield return StartCoroutine(FadeVFX(3f, 2f));
        StartCoroutine(FadeVFX(0f, 8f));
    }

    private IEnumerator FadeOutText(float timeSpeed, TextMeshProUGUI text) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeInText(float timeSpeed, TextMeshProUGUI text) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeUIControls() {
        yield return new WaitForSeconds(2f);
        wasd.CrossFadeAlpha(1f, 1f, false);
        mouse.CrossFadeAlpha(1f, 1f, false);
        shift.CrossFadeAlpha(1f, 1f, false);
        yield return new WaitForSeconds(5f);
        wasd.CrossFadeAlpha(0f, 5f, false);
        mouse.CrossFadeAlpha(0f, 5f, false);
        shift.CrossFadeAlpha(0f, 5f, false);
    }

    IEnumerator WaitAndSwitchToEndCam() {
        yield return new WaitForSeconds(7.5f);
        endCam.SetActive(true);
        soundManager.changeMusic(++currentMusic);
    }

    private IEnumerator FadeWhiteAndPainting(TextMeshProUGUI[] texts) {
        yield return new WaitForSeconds(28.5f);
        Debug.Log("On position");
        yield return StartCoroutine(FadeWhite());
        Debug.Log("Fading painting in");
        painting.CrossFadeAlpha(1f, 2f, false);
        triangle.CrossFadeAlpha(1f, 2f, false);
        foreach (TextMeshProUGUI text in texts) StartCoroutine(FadeInText(2f, text));
    }

    private IEnumerator FadeWhite() {
        endScreen.CrossFadeAlpha(1f, 2f, false);
        yield return new WaitForSeconds(2f);
    }
}
