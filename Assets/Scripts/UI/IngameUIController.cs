using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameUIController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup[] panels;

    [SerializeField]
    Button btnResume;

    [SerializeField]
    Button btnBackToMainMenu;

    enum Panel
    {
        Pause,
        Ingame
    }

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        InputHandler();
    }

    void Initialize()
    {
        HideAllPanel();
        Show(Panel.Ingame);

        btnResume.onClick.AddListener(() => {
            Time.timeScale = 1.0f;
            Hide(Panel.Pause);
        });

        btnBackToMainMenu.onClick.AddListener(() => {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        });
    }

    void InputHandler()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Toggle(Panel.Pause);
        }
    }

    void Toggle(Panel panel)
    {
        int index = (int)panel;
        bool isShow = panels[index].alpha > 0.0f;

        isShow = !isShow;
        Time.timeScale = (isShow) ? 0.0f : 1.0f;

        if (isShow) {
            Show(panel);
        }
        else {
            Hide(panel);
        }
    }

    void Show(Panel panel)
    {
        int index = (int)panel;
        panels[index].alpha = 1.0f;
        panels[index].interactable = true;
    }

    void Hide(Panel panel)
    {
        int index = (int)panel;
        panels[index].alpha = 0.0f;
        panels[index].interactable = false;
    }

    void HideAllPanel()
    {
        foreach (var panel in panels)
        {
            panel.alpha = 0.0f;
            panel.interactable = false;
        }
    }
}

