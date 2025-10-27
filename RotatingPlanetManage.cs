using UnityEngine;

public class UIRotator : MonoBehaviour
{
    [SerializeField] RectTransform target;
    [SerializeField] float rotateSpeed = 180f; // 초당 180도
    [SerializeField] float PLusMinusAngle = 45f;
    float targetAngle = 0f;

    void Update()
    {
        if (!target) return;

        // 현재 각도
        float currentAngle = target.localEulerAngles.z;
        // 부드럽게 목표 각도로 이동
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        target.localRotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    public void RotateRight()
    {
        targetAngle = Mathf.Repeat(targetAngle - PLusMinusAngle, 360f);
    }

    public void RotateLeft()
    {
        targetAngle = Mathf.Repeat(targetAngle + PLusMinusAngle, 360f);
    }

    public void RotateLeft(float absoluteAngle)
    {
        targetAngle = Mathf.Repeat(absoluteAngle, 360f);
    }
}
