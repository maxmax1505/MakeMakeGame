using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class LaserBullet : MonoBehaviour
{
    float lifeTime = 0.05f;
    RectTransform rect;

    void Awake() => rect = GetComponent<RectTransform>();

    public void Initialize(Vector2 startAnchoredPos, Vector2 endAnchoredPos, bool hit, float overshoot = 150f, float time = 0.05f, bool sniper = false)
    {
        Vector2 finalEnd = endAnchoredPos;
        lifeTime = time;

        if (!hit)
        {
            Vector2 dir = (endAnchoredPos - startAnchoredPos).normalized;
            Vector2 missDir = Quaternion.Euler(0f, 0f, Random.Range(-5f, 5f)) * dir;
            finalEnd = endAnchoredPos + missDir * overshoot;
        }

        Vector2 mid = (startAnchoredPos + finalEnd) * 0.5f;
        rect.anchoredPosition = mid;

        Vector2 direction = (finalEnd - startAnchoredPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rect.rotation = Quaternion.Euler(0f, 0f, angle);

        float length = Vector2.Distance(startAnchoredPos, finalEnd);

        if (sniper == true)
        {
            rect.sizeDelta = new Vector2(length, 3f);
        }
        else
        {
            rect.sizeDelta = new Vector2(length, rect.sizeDelta.y);
        }

        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
