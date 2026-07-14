using UnityEngine;

public class FloatingKey : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2.5f;

    [Header("Rotation")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotationSpeed = 30f;

    private Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float offsetY =
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position =
            startPosition + Vector3.up * offsetY;

        if (rotate)
        {
            transform.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}