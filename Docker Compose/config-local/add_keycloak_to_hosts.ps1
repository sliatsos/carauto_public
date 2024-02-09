#Requires -RunAsAdministrator

Write-Output "Adding keycloak to host file"
If ((Get-Content "C:\Windows\System32\drivers\etc\hosts" ) -notcontains "127.0.0.1 keycloak")
 {
        ac -Encoding UTF8  "C:\Windows\System32\drivers\etc\hosts" "127.0.0.1 keycloak" 
}
Write-Output "Added keycloak to host file"