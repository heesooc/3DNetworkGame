using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AnimationCurve))]

public class UI_DamagedEffect : MonoBehaviour
{
    public static UI_DamagedEffect Instance { get; private set; }
    public AnimationCurve ShowCurve;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        Instance = this;

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public void Show(float duration)
    {
        _canvasGroup.alpha = 1f; // Canvas 보이게
        StartCoroutine(Show_Coroutine(duration));
    }

    private IEnumerator Show_Coroutine(float duration)
    {
        float elaspedTime = 0;

        while (elaspedTime <= duration)
        {
            elaspedTime += Time.deltaTime;

            _canvasGroup.alpha = ShowCurve.Evaluate(elaspedTime / duration); 

            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }
}
