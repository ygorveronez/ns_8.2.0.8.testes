$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração CCN Telematica $date"
$filename = "..\..\LogTracking\IntegracaoCCNTelematica-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait