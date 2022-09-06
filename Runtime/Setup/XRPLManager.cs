using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ripple.Binary.Codec.Types;
using Xrpl.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class XRPLManager : ScriptableObject
{
    
    public IRippleClient client;
    private static string serverUrl = "wss://xls20-sandbox.rippletest.net:51233";
    public void InitializeChain()
    {
        client = new RippleClient(serverUrl);
        client.Connect();
    }
}