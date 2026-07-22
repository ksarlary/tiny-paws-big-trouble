using UnityEngine;

public class HealthUnitUI : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private GameObject emptyIcon;
    [SerializeField] private GameObject fullIcon;

    public void SetFull(bool isFull)
    {
        if (fullIcon != null)
        {
            fullIcon.SetActive(isFull);
        }

        if (emptyIcon != null)
        {
            emptyIcon.SetActive(!isFull);
        }
    }
}