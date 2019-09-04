using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeiling : MonoBehaviour
{
    public Renderer rend;
    public Material baseShader, fadeShader;
    public new bool enabled = true;
    void OnTriggerEnter (Collider other)
    {
        if (enabled && other.gameObject.tag == "Player")
        {
            StopCoroutine("Toggle");
            StartCoroutine(Toggle(true));
            enabled = false;
        }
    }
    void OnTriggerExit (Collider other)
    {
        if (!enabled && other.gameObject.tag == "Player")
        {
            StopCoroutine("Toggle");
            StartCoroutine(Toggle(false));
            enabled = true;
        }
    }

    IEnumerator Toggle(bool isEnabled)
    {
        float timer = 0;
        while (timer < 1)
        {
            if (isEnabled)
                rend.material.Lerp(baseShader, fadeShader, timer);
            else
                rend.material.Lerp(fadeShader, baseShader, timer);

            timer += Time.deltaTime/2;

            yield return new WaitForEndOfFrame();
        }

        if (isEnabled)
            rend.material = fadeShader;
        else
            rend.material = baseShader;
    }
}
