using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class testabi : MonoBehaviour
{
	long unitMoney = 10000000000000000;
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
        long money;
            money = 1 * unitMoney * 8 / 10;
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
    async public void PayforTicket()
	{

		// set chain
		string chain = "polygon";
		// set network
		string network = "mainnet";
		// set chainID, here we use the networkID for goerli
		string chainId = "137";
		// abi in json format
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"money\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x5c99D774519Dd8d601F438ceF2B541E5B6793Fb2";
		// value in wei 6*10^14 = 30 TWD
		string value = "10000000000000000";
		// method you want to write to
		string method = "addPlayer";
		// amount you want to change, in this case we are adding 1 to "addTotal"
		string amount = "0xa8c38c36aab9ab846a2a42899f07bd7809211ccc4746ed4677678acd99c174015c56f5f3d6533b19d1298e50ff886827f6992c7b810cb4ca8a6a5a7d7d7c309d1c";
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = { amount };
		string args = JsonConvert.SerializeObject(obj);
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);
		// send transaction
		try
		{
#if UNITY_WEBGL
			string response = await Web3GL.SendContract(method, abi, contract, args, value, "", "");
			print("sent contract " + response);
#endif
			//Connect();
		}
		catch (Exception e)
		{
			Debug.LogException(e, this);
			print("error, doesn't send contract");
		}
	}

}

