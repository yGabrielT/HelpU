using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class PauseUI : MonoBehaviour
{
    private PlayerController _playerController;
    
    private bool canAct;
    private bool isPaused;

    private CanvasGroup _pauseUICanvas;
    [SerializeField]
    private CanvasGroup _mainPanelCanvas;
    [SerializeField]
    private CanvasGroup _optionCanvas;

    [SerializeField]
    private Slider _mouseSensSlide;
    [SerializeField]
    private Slider _masterVolSlide;
    [SerializeField]
    private Slider _sfxVolSlide;
    [SerializeField]
    private Slider _ambientVolSlide;

    [SerializeField]
    private TextMeshProUGUI _mouseSensText;
    [SerializeField]
    private TextMeshProUGUI _masterVolText;
    [SerializeField]
    private TextMeshProUGUI _sfxVolText;
    [SerializeField]
    private TextMeshProUGUI _ambientVolText;
    
    void Start()
    {
        canAct = true;
        _pauseUICanvas = GetComponent<CanvasGroup>();
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    
    void Update()
    {
        if (_playerController.playerInput.pause)
        {
            _playerController.playerInput.pause = false;
            if (!isPaused && canAct)
            {
                Pause();
            }
            if(isPaused && canAct)
            {
                Resume();
            }
            
        }
        UpdateOptionsValues();
    }

    public void GoToOptions()
    {
        _mainPanelCanvas.interactable = false;
        _mainPanelCanvas.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() => 
        {
            _mainPanelCanvas.gameObject.SetActive(false);
            _optionCanvas.gameObject.SetActive(true);
            _optionCanvas.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(() => _optionCanvas.interactable = true);
            
        });
    }

    public void ReturnToMainPanel()
    {
        _optionCanvas.interactable = false;
        _optionCanvas.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() => 
        {
            _optionCanvas.gameObject.SetActive(false);
            _mainPanelCanvas.gameObject.SetActive(true);
            _mainPanelCanvas.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(() => _mainPanelCanvas.interactable = true);
        });
    }

    public void Resume()
    {
        _playerController.canControl = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPaused = false;
        canAct = false;
        _playerController.playerInput.pause = false;
        _mainPanelCanvas.gameObject.SetActive(false);
        _pauseUICanvas.DOFade(0f, .2f).SetUpdate(true).OnComplete(() => canAct = true);
    }

    public void Pause()
    {
        _playerController.canControl = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPaused = true;
        canAct = false;
        _pauseUICanvas.DOFade(1f, .2f).SetUpdate(true).OnComplete(() => { canAct = true; _pauseUICanvas.interactable = true; });
        _mainPanelCanvas.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void UpdateOptionsValues()
    {
        AudioManager.instance._masterVol = _masterVolSlide.value / 100;
        AudioManager.instance._sfxVol = _sfxVolSlide.value / 100;
        AudioManager.instance._ambientVol = _ambientVolSlide.value / 100;
        _playerController.sensivity = _mouseSensSlide.value;

        _mouseSensText.text = _mouseSensSlide.value.ToString();
        _masterVolText.text = _masterVolSlide.value.ToString();
        _sfxVolText.text = _sfxVolSlide.value.ToString();
        _ambientVolText.text = _ambientVolSlide.value.ToString();
    }
}
