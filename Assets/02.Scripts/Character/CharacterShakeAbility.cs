using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShakeAbility : MonoBehaviour
{
    public Transform TargetTransform;

    public float Duration = 0.5f;
    public float Strength = 0.2f;

   public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake_Coroutine());
    }

    private IEnumerator Shake_Coroutine()
    {
        float elaspedTime = 0;

        Vector3 startPosition = TargetTransform.localPosition;

        while (elaspedTime <= Duration)
        {
            elaspedTime += Time.deltaTime;
            Vector3 randomPosition = Random.insideUnitSphere.normalized * Strength;
            randomPosition.y = startPosition.y;
            TargetTransform.localPosition = randomPosition;
            yield return null;
        }
        TargetTransform.localPosition = startPosition;
    }
}
