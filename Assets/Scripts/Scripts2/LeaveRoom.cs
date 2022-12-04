using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StarterAssets;
using Newtonsoft.Json;

public class LeaveRoom : MonoBehaviour
{

    [SerializeField]
    private GameObject LoserLabel;

    [SerializeField]
    private GameObject WinnerLabel;

    [SerializeField]
    private TMP_Text my_text;
    int unitMoney = 6*(int)Mathf.Pow(10,14);
    void Start()
    {
        Cursor.visible = true;

        if (StateController.status == 1)
        {
            my_text.text = "YOU ARE CHAMPION !!! " + PlayerPrefs.GetInt("killAmount")/*Record.record*/;/*;*/           
        }
        else 
        {
            my_text.text = "GOT KILLED " + PlayerPrefs.GetInt("killAmount")/*Record.record*/ +  " !!! ";
        }

        Cursor.visible = true;
        Screen.lockCursor = false;
    }



    public void GoBackLobby()
    {
        withdraw();
        Destroy(KillAmount.instance.gameObject);
        SceneManager.LoadScene("Launcher");
    }

    
    async public void withdraw()
    {
        // set chain
        string chain = "ethereum";
        // set network
        string network = "goerli";
        //set rpc
        string rpc = "https://goerli.infura.io/v3/";
        // set chainID, here we use the networkID for goerli
        string chainId = "5";
        // abi in json format
        string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"money\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
        // address of contract
        string contract = "0x897140C25ACf4fAE02980624769bD815aFcB2319";
        // method you want to write to
        string method = "withdraw";
        // amount you want to change, in this case we are adding 1 to "addTotal"
        int winner = StateController.status;
        int amount = KillAmount.instance.amount;
        int money;
        //string gasPrice = await EVM.GasPrice(chain, network, rpc);
        int gasUsed = 21000;
        if (winner == 1)
        {
            money = (amount + 1) * unitMoney * 8 / 10;
        }
        else
        {
            money = amount * unitMoney * 8 / 10;
        }
        // array of arguments for contract you can also add a nonce here as optional parameter
        string[] obj = { money.ToString() };
        string args = JsonConvert.SerializeObject(obj);
        print(args);
        // create data for contract interaction
        string data = await EVM.CreateContractData(abi, method, args);
        print(data);
//#if UNITY_WEBGL
//        // send transaction
//        string response = await Web3GL.SendContract(method, abi, contract, args, "0", "", "");
//        // display response in game
//        print(response);
//#endif
    }

    public void Open() 
    {
        gameObject.SetActive(true);
    }
}
