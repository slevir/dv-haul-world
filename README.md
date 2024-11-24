# HaulWorld

<!-- ABOUT THE PROJECT -->

## About The Mod

This is a mod for the game Derail Valley which disables shunting jobs and allows to generate a lot more of Freight
Haul jobs and Logistic Haul jobs instead.

This goal is achieved by two major changes:
- Disabling the generation of shunting jobs in principle
- Allowing freight hauls to start (and optionally end) on storage tracks

Since the game needs to reserve track space to generate jobs, doing this results in a lot more Freight Haul jobs, with
some Logistic Hauls mixed in.

## Settings
### Minimum job train length
Pretty self-explanatory, the game won't generate jobs shorter than this value. Useful if you want longer trains.
Default is `3`.

### Allow multiple freight hauls per track
Normally the game only generates one job per input and output track. This is fine if you want to avoid shunting at all
costs, but you also can't have more jobs than tracks! For a station like SM, which only has 2 input tracks, this means
you won't be able to bring more than 2 jobs at any single time. Enabling this allows generating multiple jobs for a
single track, as well generating FH jobs ending on storage tracks (as it's hard to guarantee them to be empty otherwise).
Enabling this also removes the guarantee that job destination track will be empty. Default is enabled.

### Generate jobs on input tracks too!
I've decided to leave input tracks out of job generation by default, as it might make moving around station difficult
otherwise. However, if you want *even more* jobs, enable this, and jobs will generate with cargo starting on Input tracks
too. Enabling this also removes the guarantee that job destination track will be empty. Default is disabled.


## Building

Run `dotnet build` for a Debug build or `dotnet build -c Release` for a Release build.

### References Setup

After cloning the repository, some setup is required in order to successfully build the mod DLLs. You will need to create a new `Directory.Build.targets` file to specify your local reference paths. This file will be located in the main directory, next to HaulWorld.sln.

Below is an example of the necessary structure. When creating your targets file, you will need to replace the reference paths with the corresponding folders on your system. Make sure to include semicolons **between** each of the paths and no semicolon after the last path. Also note that any shortcuts you might use in file explorer—such as %ProgramFiles%—won't be expanded in these paths. You have to use full, absolute paths.
```xml
<Project>
	<PropertyGroup>
		<ReferencePath>
			C:\Program Files (x86)\Steam\steamapps\common\Derail Valley\DerailValley_Data\Managed\
		</ReferencePath>
		<AssemblySearchPaths>$(AssemblySearchPaths);$(ReferencePath);</AssemblySearchPaths>
	</PropertyGroup>
</Project>
```

## Packaging

To package a build for distribution, you can run the `package.ps1` PowerShell script in the root of the project. 
If no parameters are supplied, it will create a .zip file ready for distribution in the dist directory. 
A post build event is configured to run this automatically after each successful Release build.

Linux: `pwsh ./package.ps1`
Windows: `powershell -executionpolicy bypass .\package.ps1`


### Parameters

Some parameters are available for the packaging script.

#### -NoArchive

Leave the package contents uncompressed in the output directory.

#### -OutputDirectory

Specify a different output directory.
For instance, this can be used in conjunction with `-NoArchive` to copy the mod files into your Derail Valley installation directory.

<!-- LICENSE -->

## License

Source code is distributed under the MIT license.
See [LICENSE](https://github.com/slevir/dv-haul-world) for more information.

## Attribution

Derail Valley Modding for their excellent mod template, which this mod is based on.
The template is available at https://github.com/derail-valley-modding/template-umm.

## Additional thanks
Tank677, the author of [Job Picker mod](https://www.nexusmods.com/derailvalley/mods/771), for demonstrating that
my idea is doable.

DV modding community, for providing a lot of excellent mods I use both within the game and as reference for making this one.
