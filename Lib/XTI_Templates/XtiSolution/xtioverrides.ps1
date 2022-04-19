
function Get-Domain {
    param(
        $EnvName
    )
    if($EnvName -eq "Development")
    {
        $Domain = "development.example.com"
    }
    elseif($EnvName -eq "Test")
    {
        $Domain = "test.example.com"
    }
    elseif($EnvName -eq "Staging")
    {
        $Domain = "staging.example.com"
    }
    elseif($EnvName -eq "Production")
    {
        $Domain = "webapps.example.com"
    }
    return $Domain
}

function Get-SiteName {
    param(
        $EnvName
    )
    if($EnvName -eq "Development")
    {
        $SiteName = "development"
    }
    elseif($EnvName -eq "Test")
    {
        $SiteName = "test"
    }
    elseif($EnvName -eq "Staging")
    {
        $SiteName = "staging"
    }
    elseif($EnvName -eq "Production")
    {
        $SiteName = "webapps"
    }
    return $SiteName
}

function Get-DestinationMachine {
    param(
        $EnvName
    )
    if($EnvName -eq "Development")
    {
        $DestinationMachine = ""
    }
    else
    {
        $DestinationMachine = "server.example.com"
    }
    return $DestinationMachine
}
