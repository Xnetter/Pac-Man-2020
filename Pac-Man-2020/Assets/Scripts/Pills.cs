using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pills : MonoBehaviour
{
    public bool isPortal;

    public bool isPellet;
    public bool isLargePellet;
    private bool consumed;

    public GameObject portalReceiver;

    public bool Consumed { get => consumed; set => consumed = value; }
}
