# LYNC-Unity-AA-SDK
LYNC AA SDK is a no-code Unity SDK allowing game developers, Game studios, Indie Game Developers, and more to Integrate Biconomy Account Abstraction inside their games without worrying about Web3 Complexities.
Supported Platforms: PC (MacOS and Windows) and Mobile (Android and iOS).

## Download the SDK
Download the SDK: https://github.com/LYNC-WORLD/LYNC-Unity-AA/releases/

## Get your API Key
Please get your API key before downloading the SDK from [here](https://www.lync.world/form.html)

## Installation
Import the SDK .unitypackage file to your project. or simply drag and drop .unitypackage file to your project.

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/f7d176b5-2871-44d1-b121-bc43a4ecbbbc)


Once the Account Abstraction SDK package has finished importing into your Unity project, you can begin integrating it into your game.
The Folder structure looks like this

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/44828c75-a422-4287-966a-1899571ac051)

## Integrating AA SDK in Unity

There are 2 Example Projects present in the SDK:
Assets -> LYNC-AA-SDK -> Example / Example-2

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/e134fe59-a641-46c4-8034-52d79fa8930e)

You can find the example scene in the folders. Simply pass the API key in lyncManager GameObject.
To test, Build and Run after adding this scene in (Scene in Build).

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/08d073f2-a0db-449d-a283-ad5b0b5db5e3)

## Setup the Project

To use LYNC Manager Prefab, it needs to be attached to the first scene. This will serve as the starting point for your project.
In LYNC Manager Prefab, be sure to provide the following details:
1. LYNC API Key ([The API Key can be generated from here](https://lync.world/form.html))
2. Choose chain
3. Pass in the Dapp API Key ([The API key can be generated from the Biconomy Dashboard](https://dashboard.biconomy.io/))
4. Web3 Auth Client ID ([The API key can be generated from the Web3 Auth Dashboard](https://dashboard.web3auth.io/login))
5. Pass a deep link name (example: lync/gameName etc.)

## Integrating Login or Transaction Layer via Account Abstraction in Unity

Login and Transactions can be done once the action is triggered.
The Sample Code for Login can be found at LoginTransactionExample.cs and ExampleLogin.cs

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/7646ff31-8065-4229-98af-af758bf97500)

### Note: Make sure to Import LYNC.

```
using LYNC;
using LYNC.Wallet;
```

### Example (Event Trigger):

LYNC ready Should be a function which has an argument of type "LyncManager"

```
LyncManager.onLyncReady += LyncReady;

private void LyncReady(LyncManager Lync)
    {
        // Once LYNC is ready, you can do any steps like Login, Logout, Transactions etc.
    }
```

### To Login:

```
Lync.WalletAuth.ConnectWallet((wallet) =>
{
    addressTxt.text = "Wallet Address: " + wallet.publicAddress;
    loginDateTxt.text = "Login Date: " + wallet.loginDate.ToString();
});
```

To Logout:

```
Lync.WalletAuth.Logout();
```

### To do transactions:

To do transactions, TokenExample.cs and LoginTransactionExample.cs can be taken as a reference.

Pass in the Contract Address and Function Name Example: MintNFT(). MintNFT(unit256 id, unit256 amount)

Args are not compulsory parameters, but if the function accepts any argument, make sure to pass them.

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/dd696903-5d72-4f4b-b7ec-e386c0abd1ce)

To do it from the script:

```
LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, args, onSuccess,onError);
```

onSuccess: Once the transactions are completed, this handles what to do.
onError: If the transactions failed, this handles what to do.

### Setup Gasless Transactions with Biconomy

To enable gasless transactions via biconomy, Register a new paymaster with version 1.0.1 on the [Biconomy Dashboard](https://dashboard.biconomy.io/)https://dashboard.biconomy.io/. 

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/b73988dc-e456-4129-b009-cfb7906235b3)

Once done, Get the API Key and Pass it into the LYNC Manager.

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/bf226037-8e5b-4f63-94c1-8753866b1f5d)

Setup the gas tank, in Policies create a new one and pass the contract address

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/a4eaa9a8-ee7c-47b1-a4d6-e2ee2ffc063d)

That's it, now you can do the gasless transaction on this contract address passing in the LYNC AA SDK.


