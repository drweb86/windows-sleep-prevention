$version=1.0

Clear-Host
$ErrorActionPreference = "Stop"

class BuildInfo {
    [string]$CoreRuntimeWindows

    BuildInfo(
		[string]$CoreRuntimeWindows) {
        $this.CoreRuntimeWindows = $CoreRuntimeWindows
    }
}

$platforms = New-Object System.Collections.ArrayList
[void]$platforms.Add([BuildInfo]::new("win-x64"))
[void]$platforms.Add([BuildInfo]::new("win-arm64"))

Set-Location -Path ./src/WindowsSleepPrevention
ForEach ($platform in $platforms)
{
	Write-Output "Platform: $($platform.CoreRuntimeWindows)"

	Write-Output "Publish..."
	& dotnet publish "/p:InformationalVersion=$version" `
		"/p:VersionPrefix=$version" `
		"/p:Version=$version" `
		"/p:AssemblyVersion=$version" `
		"--runtime=$($platform.CoreRuntimeWindows)" `
		/p:Configuration=Release `
		"/p:PublishDir=../../Output/$($platform.CoreRuntimeWindows)" `
		/p:PublishReadyToRun=false `
		/p:RunAnalyzersDuringBuild=False `
		--self-contained true `
		--property WarningLevel=0
	if ($LastExitCode -ne 0)
	{
		Write-Error "Fail." 
		Exit 1
	}
}

Set-Location -Path ../..

Compress-Archive -Path Output\* -DestinationPath WindowsSleepPrevention-Release-$version.zip