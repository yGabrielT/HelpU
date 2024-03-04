using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStaminaUI : MonoBehaviour
{
    private PlayerController _player;
    public GameObject stamina3DUI;
    private float _startStaminaYScale;
    public GameObject Ground3DUI;
    private float _startGroundYScale;

    void Start()
    {
        _player = GetComponent<PlayerController>();
        _startStaminaYScale = stamina3DUI.transform.localScale.y;
        _startGroundYScale = Ground3DUI.transform.localScale.y;

    }

    // Update is called once per frame
    void Update()
    {
        ManageStaminaUI();
        ManageGroundUI();



    }

    private void ManageStaminaUI()
    {
       
        //float realStaminaVal = remap(staminaLowValue, 0, _player._maxStamina, 0, _startStaminaYScale);
        float realStaminaVal = remap(_player._stamina, 0, _player._maxStamina, 0, _startStaminaYScale);

        if (_player._stamina >= 0)
        {

            stamina3DUI.transform.DOScale(new Vector3(stamina3DUI.transform.localScale.x, realStaminaVal, stamina3DUI.transform.localScale.z), .1f).SetEase(Ease.InOutCirc);
        }

        if (stamina3DUI.transform.localScale.y <= 0.05f)
        {
            stamina3DUI.SetActive(false);
        }
        else
        {
            stamina3DUI.SetActive(true);
        }
    }

    private void ManageGroundUI()
    {
       
        //float realStaminaVal = remap(staminaLowValue, 0, _player._maxStamina, 0, _startStaminaYScale);
        float realgroundVal = remap(_player._groundTimer, 0, _player._maxGroundTimer, 0, _startGroundYScale);

        if (_player._groundTimer >= 0)
        {

            Ground3DUI.transform.DOScale(new Vector3(Ground3DUI.transform.localScale.x, realgroundVal, Ground3DUI.transform.localScale.z), .1f).SetEase(Ease.InOutCirc);
        }

        if (Ground3DUI.transform.localScale.y <= 0.05f)
        {
            Ground3DUI.SetActive(false);
        }
        else
        {
            Ground3DUI.SetActive(true);
        }
    }


    private float remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
}
