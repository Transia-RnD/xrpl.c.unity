using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Xrpl.Unity;
using Xrpl.Wallet;
using Ripple.Keypairs;
using Xrpl.Client;
using Xrpl.Client.Model.Account;
using Xrpl.Client.Requests.Account;
using Xrpl.Client.Model.Transaction;
using Xrpl.Client.Models.Methods;
using Xrpl.Client.Models.Transactions;


public class SampleExample : MonoBehaviour
{
    private static IRippleClient client;
    private static TxSigner wallet;
    private static string serverUrl = "wss://xls20-sandbox.rippletest.net:51233";
    private static string classicAddress = "r";

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
        Seed seed = Seed.FromPassPhrase("taco main silly string happened town dollar toon").SetEd25519();
        string signorAddress = seed.KeyPair().Id();
        wallet = TxSigner.FromSecret(seed.ToString());
    }

    public void EncryptXrpWallet(string seed, string secret)
    {
        // encrypt a wallet
        string encryptedSeed = AESGCM.SimpleEncryptWithPassword(
            seed.ToString(),
            secret
        );
        Debug.WriteLine(encryptedSeed);
    }

     public void DecryptXrpWallet(string seed, string secret)
    {
        // decrypt a wallet
        string encryptedSeed = AESGCM.SimpleEncryptWithPassword(
            seed.ToString(),
            secret
        );
        Debug.WriteLine(encryptedSeed);
    }

    public async Task GetAccountInfo()
    {
        // look up account info
        AccountInfo accountInfo = await client.AccountInfo(classicAddress);
        decimal currencyTotal = (decimal)accountInfo.AccountData.Balance.ValueAsXrp;
        client.Disconnect();
        Debug.WriteLine(currencyTotal);
    }

    public void XrplClient() {
        IRippleClient client = new RippleClient("wss://s.altnet.rippletest.net:51233");
        client.Connect();
        Debug.WriteLine(client);
    }

    public void XrplWallet() {
        TxSigner signer = TxSigner.FromSecret(seed);
        Debug.WriteLine(signer);
        // pub_key: ED46949E414A3D6D758D347BAEC9340DC78F7397FEE893132AAF5D56E4D7DE77B0
        // priv_key: -HIDDEN-
        // classic_address: rG5ZvYsK5BPi9f1Nb8mhFGDTNMJhEhufn6
    }

    public void XrplKeypairs() 
    {
        Seed seed = Seed.FromRandom();
        KeyPair pair = seed.KeyPair();
        string publicKey = pair.Id();
        string privateKey = seed.ToString();
        Debug.WriteLine("Here's the public key:");
        Debug.WriteLine("Here's the public key:");
        Debug.WriteLine(publicKey);
        Debug.WriteLine("Here's the private key:");
        Debug.WriteLine(privateKey);
        Debug.WriteLine("Store this in a secure place!");
        // Here's the public key:
        // ED3CC1BBD0952A60088E89FA502921895FC81FBD79CAE9109A8FE2D23659AD5D56
        // Here's the private key:
        // EDE65EE7882847EF5345A43BFB8E6F5EEC60F45461696C384639B99B26AAA7A5CD
        // Store this in a secure place!
    }

    public void SerializeAndSign() 
    {
        AccountInfo accountInfo = await client.AccountInfo("rwEHFU98CjH59UX2VqAgeCzRFU9KVvV71V");

        // prepare the transaction
        // the amount is expressed in drops, not XRP
        // see https://xrpl.org/basic-data-types.html#specifying-currency-amounts
        IPaymentTransaction paymentTransaction = new PaymentTransaction();
        paymentTransaction.Account = "rwEHFU98CjH59UX2VqAgeCzRFU9KVvV71V";
        paymentTransaction.Destination = "rEqtEHKbinqm18wQSQGstmqg9SFpUELasT";
        paymentTransaction.Amount = new Currency { ValueAsXrp = 1 };
        paymentTransaction.Sequence = accountInfo.AccountData.Sequence;

        // sign the transaction
        TxSigner signer = TxSigner.FromSecret("xxxxxxx");  //secret is not sent to server, offline signing only
        SignedTx signedTx = signer.SignJson(JObject.Parse(paymentTransaction.ToJson()));

        // submit the transaction
        SubmitBlobRequest request = new SubmitBlobRequest();
        request.TransactionBlob = signedTx.TxBlob;

        Submit result = await client.SubmitTransactionBlob(request);
    }

    public void LedgerFee() {
        Fee fee = await client.Fees();
        Debug.WriteLine(fee);
        // 10
    }
}