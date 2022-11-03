using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class testabi : MonoBehaviour
{
	async public void PayforTicket()
	{
		
		// set chain
		string chain = "ethereum";
		// set network
		string network = "goerli";
		// set chainID, here we use the networkID for goerli
		string chainId = "5";
		// abi in json format
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unitWei\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"winner\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"killAmount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x063627985D3ff94Ef1a045A4ED72bF946F4Af3aA";
		// value in wei 6*10^14 = 30 TWD
		string value = "600000000000000";
		// method you want to write to
		string method = "addPlayer";
		// amount you want to change, in this case we are adding 1 to "addTotal"
		string amount = "0xa8c38c36aab9ab846a2a42899f07bd7809211ccc4746ed4677678acd99c174015c56f5f3d6533b19d1298e50ff886827f6992c7b810cb4ca8a6a5a7d7d7c309d1c";
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = { amount };
		string args = JsonConvert.SerializeObject(obj);
		print(args);
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);
		// send transaction
		//string response = await Web3GL.SendContract(method, abi, contract, args, value, "", "");
		//print(response);
		// display response in game
	}

	async public void withdraw() 
	{
		// set chain
		string chain = "ethereum";
		// set network
		string network = "goerli";
		// set chainID, here we use the networkID for goerli
		string chainId = "5";
		// abi in json format
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unitWei\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"winner\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"killAmount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x063627985D3ff94Ef1a045A4ED72bF946F4Af3aA";
		// method you want to write to
		string method = "withdraw";
		// amount you want to change, in this case we are adding 1 to "addTotal"
		string winner = StateController.status.ToString();
		string amount = KillAmount.instance.amount.ToString();
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = {winner,amount};
		string args = JsonConvert.SerializeObject(obj);
		print(args);
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);
		// send transaction
		//string response = await Web3GL.SendContract(method, abi, contract, args, "0", "", "");
		// display response in game
		//print(response);
	}

}

