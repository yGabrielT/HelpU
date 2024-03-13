using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player;
using static UnityEditor.Experimental.GraphView.GraphView;
public class ChangeOxygenUi : MonoBehaviour
{
    public TextMeshProUGUI OxygenTextUI;
    public Image OxygenGraphicUI;
    public CanvasGroup CanvasGroupOxygen;
    public RectTransform OxygenGraphicHolderUI;
    public float oxygenTimerDivider = 2f;
    [HideInInspector]public float oxygenAmount;
    public bool isLosingOxygen;
    private bool hasLostHalf;
    private PlayerController _player;

    void Start()
    {

        _player = GetComponent<PlayerController>();
        oxygenAmount = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isLosingOxygen && oxygenAmount >= 0)
        {
            oxygenAmount -= Time.deltaTime / oxygenTimerDivider;
            OxygenGraphicUI.fillAmount = ChangeStaminaUI.remap(oxygenAmount,0,100,0,1);
        }
        if(oxygenAmount <= 50f && !hasLostHalf)
        {
            hasLostHalf = true;
            OxygenTextUI.DOColor(Color.red, .5f).SetEase(Ease.InOutCirc).OnComplete(() => OxygenTextUI.DOColor(Color.blue, .5f)).SetEase(Ease.InOutCirc).SetLoops(-1);
            OxygenGraphicUI.DOColor(Color.red, .5f).SetEase(Ease.InOutCirc);
            OxygenGraphicHolderUI.DOAnchorPos(new Vector2(32.2001f, 343f), 1f).SetEase(Ease.OutBack);
        }
        if(oxygenAmount <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _player.ManageControl(false);
            CanvasGroupOxygen.gameObject.SetActive(true);
            Time.timeScale = 0f;
            CanvasGroupOxygen.DOFade(1f, 0.2f).SetUpdate(true);
        }
    }

    public void GoToFirstCheckpoint()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        CanvasGroupOxygen.DOFade(0f, 0.2f).SetUpdate(true).SetDelay(.2f).OnComplete(() => CanvasGroupOxygen.gameObject.SetActive(false));
        Time.timeScale = 1f;
        _player._lastCheckPoint = new Vector3(-62.43f, 4.03f, -38.22f);
        _player.GoToCheckpoint();
        _player.ManageControl(true);
        oxygenAmount = 100f;
        hasLostHalf = false;
        OxygenGraphicUI.color = Color.white;
        OxygenGraphicHolderUI.anchoredPosition = new Vector2(32.2001f, 393f);
    }

    public void ManageOxygenLoss(bool valueBool)
    {
        isLosingOxygen = valueBool;
    }
}
