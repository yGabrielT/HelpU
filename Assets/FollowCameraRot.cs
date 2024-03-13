using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraRot : MonoBehaviour
{
    public GameObject camHolder;

    [SerializeField]
    private float lightSmoothFollow;
    
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, camHolder.transform.rotation, lightSmoothFollow * Time.deltaTime);
        transform.position = camHolder.transform.position;
    }
}
