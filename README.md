# StructuringNotifications

Place short description here, prerequisites and build instructions

### Prerequisites
1. Important to run next command once from IDE Terminal _(Rider:(Ctrl+Alt+1), Visual Studio:(Ctrl+` or View>Terminal))_

_Windows_(For linux and Mac look [here](https://github.com/microsoft/artifacts-credprovider#setup))
```powershell
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```
It will install Azure Artifacts [CredProvider](https://github.com/microsoft/artifacts-credprovider) to your environment and you can use `dotnet restore` from Terminal for restore packages from Stenn private nuget repository
2. After it run next command and follow instructions from console to authenticate
```powershell
dotnet restore --interactive
```
_All these steps need for successful restoring packages from private Nuget repository._