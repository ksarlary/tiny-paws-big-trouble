using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies the intended settings panel layout at runtime.
/// Attach this component to the SettingsPanel GameObject.
/// </summary>
[DisallowMultipleComponent]
public class SettingsPanelLayoutFixer : MonoBehaviour
{
    private const float SliderWidth = 235f;
    private const float SliderHeight = 58f;

    private void Start()
    {
        RectTransform optionsContainer = FindRectTransform("OptionsContainer");
        if (optionsContainer != null)
        {
            SetCenteredRect(optionsContainer, 480f, 250f, -35f);

            VerticalLayoutGroup verticalLayout =
                GetOrAddVerticalLayoutGroup(optionsContainer.gameObject);
            verticalLayout.spacing = 20f;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = true;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
        }

        ConfigureOptionRow("OptionsContainer/Option_MusicVolume");
        ConfigureOptionRow("OptionsContainer/Option_SFXVolume");
        ConfigureOptionRow("OptionsContainer/Option_Fullscreen");

        ConfigureLabel("OptionsContainer/Option_MusicVolume/Label_Music");
        ConfigureLabel("OptionsContainer/Option_SFXVolume/Label_SFX");
        ConfigureLabel("OptionsContainer/Option_Fullscreen/Label_Fullscreen");

        ConfigureSlider("OptionsContainer/Option_MusicVolume/Slider_Music");
        ConfigureSlider("OptionsContainer/Option_SFXVolume/Slider_SFX");
        ConfigureFullscreenToggle(
            "OptionsContainer/Option_Fullscreen/Toggle_Fullscreen");

        // Rebuild immediately so the corrected layout is visible this frame.
        if (optionsContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(optionsContainer);
        }
    }

    private void ConfigureOptionRow(string path)
    {
        RectTransform row = FindRectTransform(path);
        if (row == null)
        {
            return;
        }

        SetLayoutElement(row.gameObject, 450f, 62f);

        HorizontalLayoutGroup layout =
            GetOrAddHorizontalLayoutGroup(row.gameObject);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private void ConfigureLabel(string path)
    {
        RectTransform label = FindRectTransform(path);
        if (label == null)
        {
            return;
        }

        SetLayoutElement(label.gameObject, 155f, 50f);

        TMP_Text labelText = label.GetComponent<TMP_Text>();
        if (labelText == null)
        {
            Debug.LogWarning(
                $"SettingsPanelLayoutFixer: '{path}' has no TMP_Text component.",
                label.gameObject);
            return;
        }

        labelText.fontSize = 26f;
        labelText.alignment = TextAlignmentOptions.MidlineLeft;
    }

    private void ConfigureSlider(string path)
    {
        RectTransform slider = FindRectTransform(path);
        if (slider == null)
        {
            return;
        }

        SetLayoutElement(slider.gameObject, SliderWidth, SliderHeight);

        RectTransform background = FindRectTransform(path + "/Background");
        if (background != null)
        {
            SetStretchHorizontalMiddle(background, 41f);
            SetImagePreserveAspect(background.gameObject, false);
        }

        RectTransform fillArea = FindRectTransform(path + "/Fill Area");
        if (fillArea != null)
        {
            SetFullStretch(fillArea, 0f, 0f, 8f, 8f);
        }

        RectTransform handleSlideArea =
            FindRectTransform(path + "/Handle Slide Area");
        if (handleSlideArea != null)
        {
            SetFullStretch(handleSlideArea, 0f, 0f, 0f, 0f);
        }

        RectTransform handle =
            FindRectTransform(path + "/Handle Slide Area/Handle");
        if (handle != null)
        {
            SetSliderHandleSize(handle, 48f);
            SetImagePreserveAspect(handle.gameObject, true);
        }
    }

    private void ConfigureFullscreenToggle(string path)
    {
        RectTransform toggle = FindRectTransform(path);
        if (toggle == null)
        {
            return;
        }

        SetLayoutElement(toggle.gameObject, 60f, 58f);

        RectTransform background = FindRectTransform(path + "/Background");
        if (background != null)
        {
            SetFixedSize(background, 40f, 40f);
            SetImagePreserveAspect(background.gameObject, true);
        }

        RectTransform checkmark =
            FindRectTransform(path + "/Background/Checkmark");
        if (checkmark != null)
        {
            SetFixedSize(checkmark, 40f, 40f);
            SetImagePreserveAspect(checkmark.gameObject, true);
        }
    }

    private RectTransform FindRectTransform(string path)
    {
        Transform child = transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning(
                $"SettingsPanelLayoutFixer: Could not find child '{path}'.",
                gameObject);
            return null;
        }

        RectTransform rectTransform = child as RectTransform;
        if (rectTransform == null)
        {
            Debug.LogWarning(
                $"SettingsPanelLayoutFixer: '{path}' is not a RectTransform.",
                child.gameObject);
        }

        return rectTransform;
    }

    private static LayoutElement GetOrAddLayoutElement(GameObject target)
    {
        LayoutElement layoutElement = target.GetComponent<LayoutElement>();
        return layoutElement != null
            ? layoutElement
            : target.AddComponent<LayoutElement>();
    }

    private static HorizontalLayoutGroup GetOrAddHorizontalLayoutGroup(
        GameObject target)
    {
        HorizontalLayoutGroup layout = target.GetComponent<HorizontalLayoutGroup>();
        return layout != null
            ? layout
            : target.AddComponent<HorizontalLayoutGroup>();
    }

    private static VerticalLayoutGroup GetOrAddVerticalLayoutGroup(
        GameObject target)
    {
        VerticalLayoutGroup layout = target.GetComponent<VerticalLayoutGroup>();
        return layout != null
            ? layout
            : target.AddComponent<VerticalLayoutGroup>();
    }

    private static void SetLayoutElement(
        GameObject target,
        float preferredWidth,
        float preferredHeight)
    {
        LayoutElement layoutElement = GetOrAddLayoutElement(target);
        layoutElement.preferredWidth = preferredWidth;
        layoutElement.preferredHeight = preferredHeight;
    }

    private static void SetCenteredRect(
        RectTransform rectTransform,
        float width,
        float height,
        float anchoredPositionY)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = new Vector2(0f, anchoredPositionY);
    }

    private static void SetStretchHorizontalMiddle(
        RectTransform rectTransform,
        float height)
    {
        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = new Vector2(0f, -height * 0.5f);
        rectTransform.offsetMax = new Vector2(0f, height * 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void SetFullStretch(
        RectTransform rectTransform,
        float left,
        float right,
        float top,
        float bottom)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }

    private static void SetFixedSize(
        RectTransform rectTransform,
        float width,
        float height)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    private static void SetSliderHandleSize(
        RectTransform rectTransform,
        float size)
    {
        // The Slider component controls the X anchors. Only center the handle
        // vertically so the paw remains on top of the wooden bar.
        rectTransform.anchorMin =
            new Vector2(rectTransform.anchorMin.x, 0.5f);
        rectTransform.anchorMax =
            new Vector2(rectTransform.anchorMax.x, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition =
            new Vector2(rectTransform.anchoredPosition.x, 0f);
        rectTransform.sizeDelta = new Vector2(size, size);
    }

    private static void SetImagePreserveAspect(
        GameObject target,
        bool preserveAspect)
    {
        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning(
                $"SettingsPanelLayoutFixer: '{target.name}' has no Image component.",
                target);
            return;
        }

        image.type = Image.Type.Simple;
        image.preserveAspect = preserveAspect;
    }
}
