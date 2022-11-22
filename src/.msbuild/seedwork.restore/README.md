## Using restore seedwork msbuild packages

Seedwork MSBuild packages allows to use correct version of Seedwork packages and its dependency packages by setting only one msbuild property - __SeedworkVersion__ 
With this modification solution can reuse __packages.versions__ files for Seedwork packages and its dependency packages from Seedwork.MSBuild.Packages. If it necessary packages can be overrided or added in `nuget.packages.version.targets` file.

This folder contains client part that must be copied to solution for using `Seedwork.MSBuild.Packages`

#### *Prerequisites*
1. Important to run next command once from IDE Terminal _(Rider:(*Ctrl+Alt+1*), Visual Studio:(*Ctrl+`* or View>Terminal))_

_Windows_(For linux and Mac look [here](https://github.com/microsoft/artifacts-credprovider#setup))
```powershell
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```
It will install Azure Artifacts [CredProvider](https://github.com/microsoft/artifacts-credprovider) to your environment and you can use `dotnet restore` from Terminal for restore packages from Stenn private nuget repository
2. After it run next command and follow instructions from console to authenticate
```powershell
dotnet restore --interactive
```

3. For using seedwork nuget versions file and seedwork deps nuget versions file the solution must be already converted to use __nuget.packages.version__ file as a storage for PackageReference metadata for the whole solution



### Integration

1. Copy this folder to folder with __nuget.packages.version__ file. Normally this is a __%SLNDIR%\.msbuild__ folder.
2. Add `seedwork.props` file. Add msbuild property `SeedworkVersion`. Replace `15.0.0.0` with desired Seedwork version
```xml
<!--        Actual version of Seedwork packages-->
    <SeedworkVersion>15.0.0.0</SeedworkVersion>
```
3. `Directory.Build.props` file. Add next code to it. All file routing uses __%SLNDIR%\.msbuild__ as folder for MSBuild files
```xml
<Import Project="$(MSBuildThisFileDirectory).msbuild\seedwork.restore\seedwork.restore.props" />
```
__IMPORTANT:__ This code must be inserted __AFTER__ `build.props` file import.
4. `Directory.Build.targets` file. Add next code to it.
```xml
<Import Project="$(MSBuildThisFileDirectory).msbuild\seedwork.restore\seedwork.restore.targets"/>
```
__IMPORTANT:__ This code must be inserted __BEFORE__ `nuget.packages.version` file import.