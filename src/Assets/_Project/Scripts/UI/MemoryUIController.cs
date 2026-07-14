using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MemoryUIController : MonoBehaviour
{
    public static MemoryUIController Instance { get; private set; }

    public static bool IsMemoryOpen { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject memoryRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    private Action closeCallback;

    private void Awake()
    {
        Instance = this;

        IsMemoryOpen = false;

        if (memoryRoot != null)
        {
            memoryRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsMemoryOpen)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        bool closePressed =
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame;

        if (closePressed)
        {
            CloseMemory();
        }
    }

    public void ShowMemory(
        string title,
        string text,
        Action onClosed)
    {
        if (memoryRoot == null)
        {
            Debug.LogError(
                "MemoryUIController: Memory Root is not assigned.",
                this
            );

            return;
        }

        titleText.text = title;
        bodyText.text = text;

        closeCallback = onClosed;

        IsMemoryOpen = true;

        memoryRoot.SetActive(true);

        Time.timeScale = 0f;
    }

    private void CloseMemory()
    {
        if (!IsMemoryOpen)
        {
            return;
        }

        IsMemoryOpen = false;

        memoryRoot.SetActive(false);

        Time.timeScale = 1f;

        Action callback = closeCallback;
        closeCallback = null;

        callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        IsMemoryOpen = false;
    }
}