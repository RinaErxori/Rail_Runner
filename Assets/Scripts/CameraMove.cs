using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // ������ �� ��������� ������
    [SerializeField] private float followSpeed = 5f; // �������� ���������� ������

    [Header("Position Settings")]
    [SerializeField] private float yPosition = 10f; // ������������� ������� Y
    [SerializeField] private float zPosition = -10f; // ������������� ������� Z
    [SerializeField] private float xofset = 10f; // ������������� ������� Z

    private Vector3 _targetPosition;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned for CameraFollowX!");
            enabled = false;
            return;
        }

        // �������������� ��������� ������� ������
        _targetPosition = new Vector3(target.position.x+xofset, yPosition, zPosition);
        transform.position = _targetPosition;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ��������� ������ X-����������, �������� Y � Z
        _targetPosition.x = target.position.x+xofset;

        // ������� ����������� ������
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
    }

    // ����� ��� ������ ��������� ������� Y � Z (���� ����� �������� �� ����� ����)
    public void SetCameraPosition(float newY, float newZ)
    {
        yPosition = newY;
        zPosition = newZ;
        _targetPosition.y = newY;
        _targetPosition.z = newZ;
    }
}