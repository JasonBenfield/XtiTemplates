Import-Module PowershellForXti -Force

$script:xtiConfig = [PSCustomObject]@{
    RepoOwner = "JasonBenfield"
    RepoName = "HubWebApp"
    AppName = "Hub"
    AppType = "WebApp"
}

if(Test-Path ".\xti.private.ps1"){
. .\xti.Private.ps1
}

function Xti-NewVersion {
    param(
        [ValidateSet(“major”, "minor", "patch")]
        $VersionType = "minor"
    )
    $script:xtiConfig | New-BaseXtiVersion @PsBoundParameters
}

function Xti-Issues {
    param(
    )
    $script:xtiConfig | BaseXti-Issues @PsBoundParameters
}

function Xti-NewIssue {
    param(
        [Parameter(Mandatory)]
        [string] $IssueTitle,
        [switch] $Start
    )
    $script:xtiConfig | New-BaseXtiIssue @PsBoundParameters
}

function Xti-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0
    )
    $script:xtiConfig | BaseXti-StartIssue @PsBoundParameters
}

function Xti-CompleteIssue {
    param(
    )
    $script:xtiConfig | BaseXti-CompleteIssue @PsBoundParameters
}

function Xti-Build {
    param(
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Development"
    )
    $script:xtiConfig | BaseXti-BuildWebApp @PsBoundParameters
}

function Xti-Publish {
    param(
        [ValidateSet("Production", "Development")]
        [string] $EnvName="Development"
    )
    $DestinationMachine = Get-DestinationMachine -EnvName $EnvName
    $PsBoundParameters.Add("DestinationMachine", $DestinationMachine)
    $Domain = Get-Domain -EnvName $EnvName
    $PsBoundParameters.Add("Domain", $Domain)
    $SiteName = Get-SiteName -EnvName $EnvName
    $PsBoundParameters.Add("SiteName", $SiteName)
    $script:xtiConfig | BaseXti-Publish @PsBoundParameters
}

function Xti-Install {
    param(
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Development"
    )
    $DestinationMachine = Get-DestinationMachine -EnvName $EnvName
    $PsBoundParameters.Add("DestinationMachine", $DestinationMachine)
    $Domain = Get-Domain -EnvName $EnvName
    $PsBoundParameters.Add("Domain", $Domain)
    $SiteName = Get-SiteName -EnvName $EnvName
    $PsBoundParameters.Add("SiteName", $SiteName)
    $script:xtiConfig | BaseXti-Install @PsBoundParameters
}

function Add-HubDBMigrations {
    param ([Parameter(Mandatory)]$Name)
    $env:DOTNET_ENVIRONMENT="Development"
    dotnet ef --startup-project ./Tools/HubDbTool migrations add $Name --project ./Internal/XTI_HubDB.EF.SqlServer
}
