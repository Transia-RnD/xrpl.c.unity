using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
// using System.Diagnostics;
using Xrpl.Unity;
using Xrpl.XrplWallet;
using Ripple.Keypairs;
using Xrpl.Client;
using Xrpl.Client.Models.Methods;
using Xrpl.Client.Models.Transactions;
using static Xrpl.XrplWallet.Wallet;
using Xrpl.Client.Models.Common;


public class SampleExample : MonoBehaviour
{
    private static IRippleClient client;
    private static Wallet wallet;
    private static string serverUrl = "wss://hooks-testnet-v2.xrpl-labs.com";
    private static string classicAddress = "radyYfJy6M4P58S3XTGYQr5tKYbnH74DEj";

    private static string seed = "snYLsV8VWT7TKjT3cEifBQB3b1dPY";

    public void Awake()
    {
        // create a network client
        client = new RippleClient(serverUrl);
        client.Connect();
        CreateXrpWallet();
    }

    public void CreateXrpWallet()
    {
        // create a wallet on the testnet
        wallet = Wallet.FromSeed(seed);
    }

    public void EncryptXrpWallet(string seed, string secret)
    {
        // encrypt a wallet
        string encryptedSeed = AESGCM.SimpleEncryptWithPassword(
            seed.ToString(),
            secret
        );
        Debug.Log(encryptedSeed);
    }

     public void DecryptXrpWallet(string seed, string secret)
    {
        // decrypt a wallet
        string encryptedSeed = AESGCM.SimpleEncryptWithPassword(
            seed.ToString(),
            secret
        );
        Debug.Log(encryptedSeed);
    }

    public async Task GetAccountInfo()
    {
        // look up account info
        AccountInfoRequest request = new AccountInfoRequest(classicAddress);
        AccountInfo accountInfo = await client.AccountInfo(request);
        decimal currencyTotal = (decimal)accountInfo.AccountData.Balance.ValueAsXrp;
        client.Disconnect();
        Debug.Log(currencyTotal);
    }

    public void XrplClient() {
        IRippleClient client = new RippleClient("wss://hooks-testnet-v2.xrpl-labs.com");
        client.Connect();
        Debug.Log(client);
    }

    public void XrplWallet() {
        Wallet wallet = Wallet.FromSeed(seed);
        Debug.Log(wallet);
        // pub_key: ED46949E414A3D6D758D347BAEC9340DC78F7397FEE893132AAF5D56E4D7DE77B0
        // priv_key: -HIDDEN-
        // classic_address: rG5ZvYsK5BPi9f1Nb8mhFGDTNMJhEhufn6
    }

    public void XrplKeypairs() 
    {
        Wallet wallet = Wallet.Generate();
        string publicKey = wallet.PublicKey;
        string privateKey = wallet.PrivateKey;
        Debug.Log("Here's the public key:");
        Debug.Log(publicKey);
        Debug.Log("Here's the private key:");
        Debug.Log(privateKey);
        Debug.Log("Store this in a secure place!");
        // Here's the public key:
        // ED3CC1BBD0952A60088E89FA502921895FC81FBD79CAE9109A8FE2D23659AD5D56
        // Here's the private key:
        // EDE65EE7882847EF5345A43BFB8E6F5EEC60F45461696C384639B99B26AAA7A5CD
        // Store this in a secure place!
    }

    public async void SerializeAndSign() 
    {
        AccountInfoRequest request = new AccountInfoRequest(classicAddress);
        AccountInfo accountInfo = await client.AccountInfo(request);

        // prepare the transaction
        // the amount is expressed in drops, not XRP
        // see https://xrpl.org/basic-data-types.html#specifying-currency-amounts
        IPayment paymentTransaction = new Payment();
        paymentTransaction.Account = classicAddress;
        paymentTransaction.Destination = "rEqtEHKbinqm18wQSQGstmqg9SFpUELasT";
        paymentTransaction.Amount = new Currency { ValueAsXrp = 1 };
        paymentTransaction.Sequence = accountInfo.AccountData.Sequence;

        // sign the transaction
        Dictionary<string, dynamic> paymentJson = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(paymentTransaction.ToJson());
        SignatureResult signedTx = wallet.Sign(paymentJson);

        // submit the transaction
        SubmitRequest request1 = new SubmitRequest();
        request1.TxBlob = signedTx.TxBlob;

        Submit result = await client.Submit(request1);
    }

    public async void SubmitTx() 
    {
        AccountInfoRequest request = new AccountInfoRequest(classicAddress);
        AccountInfo accountInfo = await client.AccountInfo(request);

        // prepare the transaction
        // the amount is expressed in drops, not XRP
        // see https://xrpl.org/basic-data-types.html#specifying-currency-amounts
        IPayment paymentTransaction = new Payment();
        paymentTransaction.Account = classicAddress;
        paymentTransaction.Destination = "rEqtEHKbinqm18wQSQGstmqg9SFpUELasT";
        paymentTransaction.Amount = new Currency { ValueAsXrp = 1 };
        paymentTransaction.Sequence = accountInfo.AccountData.Sequence;

        // submit the transaction
        Dictionary<string, dynamic> paymentJson = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(paymentTransaction.ToJson());
        Submit result = await client.Submit(paymentJson, wallet);
    }

    public async void LedgerFee() {
        FeeRequest feeRequest = new FeeRequest();
        Fee fee = await client.Fee(feeRequest);
        Debug.Log(fee);
        // 10
    }
}