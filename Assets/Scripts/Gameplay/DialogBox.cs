using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour
{
    public GameObject panelRoot;
    public Text dialogText;
    public Button continueButton;

    private Action onContinue;

    void Awake()
    {
        if (continueButton != null) continueButton.onClick.AddListener(HandleContinue);
        Hide();
    }

    void Update()
    {
        if (panelRoot != null && panelRoot.activeSelf && Keyboard.current != null &&
            (Keyboard.current.xKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
        {
            HandleContinue();
        }
    }

    public void Show(string text, Action onContinueCallback)
    {
        if (dialogText != null) dialogText.text = text;
        onContinue = onContinueCallback;
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void HandleContinue()
    {
        if (panelRoot == null || !panelRoot.activeSelf) return;
        var callback = onContinue;
        onContinue = null;
        Hide();
        callback?.Invoke();
    }
}
