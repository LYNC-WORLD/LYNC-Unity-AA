# LYNC-Unity-AA-SDK
LYNC AA SDK is a no-code Unity SDK allowing game developers, Game studios, Indie Game Developers, and more to Integrate Biconomy Account Abstraction inside their games without worrying about Web3 Complexities.
Supported Platforms: PC (MacOS and Windows) and Mobile (Android and iOS).

## Get your API Key
Please get your API key before downloading the SDK from [here](https://www.lync.world/form.html)

## Installation
Import the SDK .unitypackage file to your project. or simply drag and drop .unitypackage file to your project.

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/c54d1e34-6086-4fca-89cc-13169ac75027)

Once the Account Abstraction SDK package has finished importing into your Unity project, you can begin integrating it into your game.
The Folder structure looks like this

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/f1d277ed-aab4-4990-978c-1e79fe231d10)

## Integrating AA SDK in Unity

There are 2 Example Projects present in the SDK:
Assets -> LYNC-AA-SDK -> Example / Example-2

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/7af242b0-38af-4c9b-b468-ac8f60653daa)

You can find the example scene in the folders. Simply pass the API key in lyncManager GameObject.
To test, Build and Run after adding this scene in (Scene in Build).

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/a495606a-2b0c-4121-9b4c-12187236dfb6)

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

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/277b600f-fd3f-48a8-8d14-2b4c954b1f6a)

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

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/9234feda-eebd-4797-a127-17e50b7fd610)

To do it from the script:

```
LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, args, onSuccess,onError);
```

onSuccess: Once the transactions are completed, this handles what to do.
onError: If the transactions failed, this handles what to do.

### Setup Gasless Transactions with Biconomy

To enable gasless transactions via biconomy, Register a new paymaster on the [Biconomy Dashboard](https://dashboard.biconomy.io/)https://dashboard.biconomy.io/. 

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/d83043e2-eb1c-4f54-9e49-57bbee6a9f8f)

Once done, Get the API Key and Pass it into the LYNC Manager.

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/070a291c-fe65-4a7b-83e8-210f895184f3)

Setup the gas tank, in Policies create a new one and pass the contract address

![image](https://github.com/LYNC-WORLD/LYNC-Unity-AA/assets/42548654/d1bec52f-9759-4cda-a118-3ae5ca39e08c)

That's it, now you can do the gasless transaction on this contract address passing in the LYNC AA SDK.


