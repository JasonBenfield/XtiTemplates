Import-Module PowershellForXti -Force

if(Test-Path ".\xti.private.ps1"){
. .\xti.private.ps1
}

function Xti-NewVersion {
    param(
        [ValidateSet(“major”, "minor", "patch")]
        $VersionType = "minor"
    )
    $Domain = Get-Domain -EnvName Production
    $PsBoundParameters.Add("Domain", $Domain)
    New-BaseXtiVersion @PsBoundParameters
}

function Xti-Issues {
    BaseXti-Issues @PsBoundParameters
}

function Xti-NewIssue {
    param(
        [Parameter(Mandatory)]
        [string] $IssueTitle,
        [switch] $Start
    )
    New-BaseXtiIssue @PsBoundParameters
}

function Xti-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0
    )
    BaseXti-StartIssue @PsBoundParameters
}

function Xti-CompleteIssue {
    param(
    )
    BaseXti-CompleteIssue @PsBoundParameters
}

function Xti-Build {
    param(
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Development"
    )
    BaseXti-Build @PsBoundParameters
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
    BaseXti-Publish @PsBoundParameters
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
    BaseXti-Install @PsBoundParameters
}