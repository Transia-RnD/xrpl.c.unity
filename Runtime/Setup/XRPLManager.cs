using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xrpl.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class XRPLManager : ScriptableObject
{
    
    public IXrplClient client;
    private static string serverUrl = "wss://hooks-testnet-v2.xrpl-labs.com";
    public async void InitializeChain()
    {
        IXrplClient client = new XrplClient(serverUrl);
        await client.Connect();
    }
}