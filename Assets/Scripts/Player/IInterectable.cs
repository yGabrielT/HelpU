using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterectable
{
    public bool wasInteracted { get; set; }
    public void Response();
}
