using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class testabi : MonoBehaviour
{
	int unitMoney = 2;
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
		string gasPrice = await EVM.GasPrice(chain, network, rpc);
		int gasUsed = 21000;
		if (winner == 1)
        {
			money = (amount + 1) * unitMoney * 8 / 10  + int.Parse(gasPrice)*gasUsed;
		}
        else
        {
			money = amount * unitMoney * 8 / 10 * int.Parse(gasPrice) * gasUsed;
		}
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = {money.ToString()};
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

}

