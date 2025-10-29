using UnityEngine;

public class UIRotator : MonoBehaviour
{
    [SerializeField] RectTransform target;
    [SerializeField] float rotateSpeed = 180f; // �ʴ� 180��
    [SerializeField] float PLusMinusAngle = 45f;
    float targetAngle = 0f;

    void Update()
    {
        if (!target) return;

        // ���� ����
        float currentAngle = target.localEulerAngles.z;
        // �ε巴�� ��ǥ ������ �̵�
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
