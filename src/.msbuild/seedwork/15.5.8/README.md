# MSBuild common imports

This folder contains MSBuild props and targets used for: 

1. Import Seedwork nuget package versions and Seedwork nuget package versions that used by Seedwork.
2. Import common csproj properties along all codebase.
3. Import common csproj checks shared along all codebase.
4. Attach Seedwork as ProjectReference instead of PackageReference.

__IMPORTANT:__ Do not change any files in this folder. This folder will updated automatically during restore seedwork.msbuild.packages. seedwork version controlled by property SeedworkVersion in build.props

*Remarks: This readme valid for local copy of Seedwork.Installer.MSBuild.Targets in app service*