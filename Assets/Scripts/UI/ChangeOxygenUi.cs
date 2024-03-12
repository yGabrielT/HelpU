using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOxygenUi : MonoBehaviour
{
    public TextMeshProUGUI OxygenTextUI;
    public Image OxygenGraphicUI;
    public RectTransform OxygenGraphicHolderUI;
    public float oxygenTimerDivider = 2f;
    [HideInInspector]public float oxygenAmount;
    public bool isLosingOxygen;
    private bool hasLostHalf;

    void Start()
    {

        
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
    }
}
