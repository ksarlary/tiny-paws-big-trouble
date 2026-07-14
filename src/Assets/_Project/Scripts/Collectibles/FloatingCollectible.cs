using UnityEngine;

public class FloatingCollectible : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotationSpeed = 20f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float verticalOffset =
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.localPosition = startPosition +
            Vector3.up * verticalOffset;

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