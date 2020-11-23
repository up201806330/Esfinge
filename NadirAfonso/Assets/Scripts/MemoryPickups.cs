using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;

public class MemoryPickups : MonoBehaviour
{
    public Image mainMenu;
    public Image wasd;
    public Image mouse;

    public VisualEffect vfx;
    float intensity = 0;

    public CinemachineFreeLook cam;
    public ThirdPersonMovement movementController;

    public GameObject firstObjects;
    public GameObject finalObjects;

    public GameObject[] memories;
    public GameObject playerCam;
    public Indicator indicator;
    public float timeout = 1000f;

    public int currentMem = 0;

    public Material playerMat;
    public Material eyesMat;
    public float blinkAmount = 1f;
    public float blinkInTime = 1f;
    public float blinkOutTime = 1f;

    private void Start() {
        foreach (GameObject mem in memories) mem.SetActive(false);
        memories[0].SetActive(true);

        firstObjects.SetActive(false);
        finalObjects.SetActive(false);

        playerMat.SetFloat("_Dissolve", 1f);
        eyesMat.SetFloat("_Dissolve", 1f);

        wasd.CrossFadeAlpha(0f, 0.001f, false);
        mouse.CrossFadeAlpha(0f, 0.001f, false);
    }

    public void StartGame() { // Called by main menu
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
        Activate(currentMem = 1);
    }

    public void Activate(int memoryIndex) {
        memories[currentMem - 1].SetActive(false);

        if (memoryIndex != 1) StartCoroutine(BlinkRoutine());

        if (memoryIndex == memories.Length - 2) {
            firstObjects.SetActive(true);
        }

        if (memoryIndex == memories.Length - 1) {
            // Fade and destroy UI indicator
            StartCoroutine(indicator.FadeTo(0f, 1.5f));
            Destroy(indicator, 1.5f);

            // Fade out sandstorm
            StartCoroutine(FadeVFX(0.4f, 5f));

            // Deactivate Camera input
            movementController.ToggleControls();
            movementController.animationStateMachine(false, false, false);

            // Activate Final objects (pyramid and stuff)
            finalObjects.SetActive(true);
        }
        else {
            intensity += 0.45f;
            StartCoroutine(FadeVFX(intensity, 1.5f));
        }
        // Sound effect

        memories[memoryIndex].SetActive(true);

        if (memoryIndex == 1) {
            StartCoroutine(SwitchCameras(memoryIndex));
            StartCoroutine(indicator.FadeTo(indicator.minAlpha, 1.5f));
            // Switch to track 1
        }
        // Switch music

        // UI indicator update
        indicator.retarget(memories[memoryIndex].transform);
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

    private IEnumerator FadeOutText(float timeSpeed, TextMeshProUGUI text) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeUIControls() {
        yield return new WaitForSeconds(2f);
        wasd.CrossFadeAlpha(1f, 1f, false);
        mouse.CrossFadeAlpha(1f, 1f, false);
        yield return new WaitForSeconds(5f);
        wasd.CrossFadeAlpha(0f, 5f, false);
        mouse.CrossFadeAlpha(0f, 5f, false);
    }
}
