$currentDate = Get-Date

configuration WebServerDSC {
    Node WebServer
    {
        WindowsFeature IIS
        {
            Ensure = 'Present'
            Name = "Web-Server"
            IncludeAllSubFeature = $true
        }
        WindowsFeature HTTPFeatures
        {
            Ensure = 'Present'
            Name = 'Web-Common-Http'
        }
        WindowsFeature DotNET45
        {
            Ensure = 'Present'
            Name = 'NET-Framework-45-Features'
            IncludeAllSubFeature = $true
        }
        WindowsFeature WebMgmtTools
        {
            Ensure = 'Present'
            Name = 'Web-Mgmt-Tools'
            IncludeAllSubFeature = $true 
        }
        WindowsFeature Security
        {
            Ensure = 'Present'
            Name = 'Web-Security'
            IncludeAllSubFeature = $true
        }
        File DSCApplyValidation
        {
            DestinationPath = 'C:\Users\sysadmin\Desktop\Validation.txt'
            Ensure = 'Present'
            Contents = 'DSC has been run on ' + $currentDate
            
        }
    }
}
