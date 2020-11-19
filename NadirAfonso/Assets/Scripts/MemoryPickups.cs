using System.Collections;
using Cinemachine;
using UnityEngine;

public class MemoryPickups : MonoBehaviour
{
    public GameObject[] memories;
    public GameObject playerCam;
    public Indicator indicator;
    public float timeout = 5f;

    public int currentMem = 0;

    public Material playerMat;
    public Material eyesMat;
    public float blinkAmount = 1f;
    public float blinkInTime = 1f;
    public float blinkOutTime = 1f;

    private void Start() {
        foreach (GameObject mem in memories) mem.SetActive(false);
        memories[0].SetActive(true);

        playerMat.SetFloat("_Dissolve", 1f);
        eyesMat.SetFloat("_Dissolve", 1f);
        StartCoroutine(BlinkTo(playerMat, "_Dissolve", 0f, 8f));
        StartCoroutine(BlinkTo(eyesMat, "_Dissolve", 0f, 8f));

        StartCoroutine(FirstMemTimeout());
    }

    public void onGameStart() { // Called by main menu
        
    }
    
    private IEnumerator FirstMemTimeout() {
        yield return new WaitForSeconds(timeout);
        memories[0].SetActive(false);
        Activate(currentMem = 1);
    }

    public void Activate(int memoryIndex) {
        if (memoryIndex != 1) StartCoroutine(BlinkRoutine());
        if (memoryIndex == memories.Length - 1) {
            StartCoroutine(indicator.FadeTo(0f, 1.5f));
            Destroy(indicator, 1.5f);
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

}
