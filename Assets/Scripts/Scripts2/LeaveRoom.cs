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
    long unitMoney = 1000000000000000000;
    void Start()
    {
        Cursor.visible = true;
        my_text.text = "YOU are suck !!!";

        if (StateController.status == 1)
        {
            my_text.text = "YOU ARE CHAMPION !!! ";           
        }
        else if (StateController.status == 0)
        {
            /*PlayerPrefs.GetInt("Status")*/
            my_text.text = "GOT KILLED !!! ";
        }
        else 
        {
            
        }
        Cursor.visible = true;
        Screen.lockCursor = false;
        PlayerPrefs.DeleteAll();
    }



    public void GoBackLobby()
    {
        withdraw();
        Destroy(KillAmount.instance.gameObject);
        Kill.killAmount = 0;
        SceneManager.LoadScene("Launcher");
    }

    
    async public void withdraw()
    {
        // set chain
        string chain = "polygon";
        // set network
        string network = "mainnet";
        //set rpc
        string rpc = "https://polygon-rpc.com";
        // set chainID, here we use the networkID for goerli
        string chainId = "137";
        // abi in json format
        string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"money\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
        // address of contract
        string contract = "0x5c99D774519Dd8d601F438ceF2B541E5B6793Fb2";
        // method you want to write to
        string method = "withdraw";
        // amount you want to change, in this case we are adding 1 to "addTotal"
        int winner = StateController.status;
        int amount = KillAmount.instance.amount;
        long money;
        print("winner: " + winner);
        print("amount: " + amount);
        if (winner == 1)
        {
            money = (amount + 1) * unitMoney * 8 / 10;
        }
        else
        {
            money = amount * unitMoney * 8 / 10;
        }
        print("money: " + money);
        // array of arguments for contract you can also add a nonce here as optional parameter
        string[] obj = { money.ToString() };
        string args = JsonConvert.SerializeObject(obj);
        print(args);
        // create data for contract interaction
        string data = await EVM.CreateContractData(abi, method, args);
        print(data);
#if UNITY_WEBGL
        // send transaction
        string response = await Web3GL.SendContract(method, abi, contract, args, "0", "", "");
        // display response in game
        print(response);
#endif
    }

    public void Open() 
    {
        gameObject.SetActive(true);
    }
}
