using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class testabi : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
		//OnSendTransaction();
		PayforTicket();
		//withdraw();
		//test();
		//test123();
	}
	async public void OnSendTransaction()
	{
		// https://chainlist.org/
		string chainId = "5"; // Goerli testnet
							  // account to send to
		string to = "0x5c99D774519Dd8d601F438ceF2B541E5B6793Fb2";
		// value in wei 6*10^14 = 30 TWD
		string value = "600000000000000";
		// data OPTIONAL
		string data = "";
		// gas limit OPTIONAL
		string gasLimit = "";
		// gas price OPTIONAL
		string gasPrice = "";
		// send transaction
		string response = await Web3Wallet.SendTransaction(chainId, to, value, data, gasLimit, gasPrice);
		print(response);
	}

	async public void test123()
    {
		// set chain
string chain = "ethereum";
		// set network
		string network = "goerli";
		// set chainID, here we use the networkID for goerli
		string chainId = "5";
		// abi in json format
		string abi = "[ { \"inputs\": [ { \"internalType\": \"uint8\", \"name\": \"_myArg\", \"type\": \"uint8\" } ], \"name\": \"addTotal\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"myTotal\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" } ]";
		// address of contract
		string contract = "0x741C3F3146304Aaf5200317cbEc0265aB728FE07";
		// method you want to write to
		string method = "addTotal";
		// amount you want to change, in this case we are adding 1 to "addTotal"
		string amount = "0xa8c38c36aab9ab846a2a42899f07bd7809211ccc4746ed4677678acd99c174015c56f5f3d6533b19d1298e50ff886827f6992c7b810cb4ca8a6a5a7d7d7c309d1c";
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = { amount };
		string args = JsonConvert.SerializeObject(obj);
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);	
		// send transaction
		string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, "", "");
		// display response in game
		print(response);
	}
	async public void PayforTicket()
	{
		// set chain
		string chain = "ethereum";
		// set network
		string network = "goerli";
		// set chainID, here we use the networkID for goerli
		string chainId = "5";
		// abi in json format
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unitWei\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"winner\",\"type\":\"bool\"},{\"internalType\":\"uint256\",\"name\":\"killAmount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x3A370E29924fF4755DfF06a16ae99E024fB4E14f";
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
		string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, "", "");
		// display response in game
		print(response);
		

		
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
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unitWei\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"winner\",\"type\":\"bool\"},{\"internalType\":\"uint256\",\"name\":\"killAmount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x3A370E29924fF4755DfF06a16ae99E024fB4E14f";
		// method you want to write to
		string method = "withdraw";
		// amount you want to change, in this case we are adding 1 to "addTotal"
		string winner = "false";
		string amount = "1";
		// array of arguments for contract you can also add a nonce here as optional parameter
		string[] obj = {winner,amount};
		string args = JsonConvert.SerializeObject(obj);
		print(args);
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);
		// send transaction
		string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, "", "");
		// display response in game
		print(response);
	}
	async public void test()
    {
		// set chain
		string chain = "ethereum";
		// set network
		string network = "goerli";
		string chainId = "5";
		// abi in json format
		string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"Charge\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"removePlayer\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unitWei\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"winner\",\"type\":\"bool\"},{\"internalType\":\"uint256\",\"name\":\"killAmount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
		// address of contract
		string contract = "0x5c99D774519Dd8d601F438ceF2B541E5B6793Fb2";
		// array of arguments for contract
		string args = "[]";

		string method = "removePlayer";
		// connects to user's browser wallet to call a transaction
		// create data for contract interaction
		string data = await EVM.CreateContractData(abi, method, args);
		print(data);
		// send transaction
		string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, "", "");
		// display response in game
		print(response);
	}
}

