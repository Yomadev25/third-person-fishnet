using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;

public class ServerConnector : MonoBehaviour
{
    [SerializeField]
    private Tugboat _tugboat;

    private void Start()
    {
        if (_tugboat)
        {
            _tugboat.StartConnection(true);
            _tugboat.StartConnection(false);
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Tugboat));
        }
    }
}
