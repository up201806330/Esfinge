using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Indicator : MonoBehaviour
{
    public Transform player;

    public Transform target = null;
    public float rotSpeed = 10f;

    CanvasGroup cg;
    public float maxAlpha = 0.8f;
    public float minAlpha = 0.2f;
    public float maxDistance = 10f;

    private void Start() {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
    }

    public void retarget(Transform t) {
        target = t;
        //StartCoroutine(FadeTo(1f, 1.5f));
    }

    void Update() {
        if (target == null) return;

        Quaternion rotation = Quaternion.LookRotation(player.position - target.position);
        rotation.z = -rotation.y;
        rotation.x = 0;
        rotation.y = 0;

        Quaternion previousRotation = GetComponent<Transform>().localRotation;
        transform.localRotation = Quaternion.RotateTowards(previousRotation, rotation * Quaternion.Euler(new Vector3(0, 0, player.eulerAngles.y)), rotSpeed * Time.deltaTime);

        float t = Vector3.Distance(player.position, target.position) / maxDistance;
        cg.alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
        float lerp = Mathf.Lerp(1f, 2.4f, t);
        GetComponent<RectTransform>().localScale = new Vector3(lerp, lerp, 1);
    }

    public IEnumerator FadeTo(float aValue, float aTime) {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            cg.alpha = Mathf.Lerp(cg.alpha, aValue, t);
            yield return null;
        }
    }

}
