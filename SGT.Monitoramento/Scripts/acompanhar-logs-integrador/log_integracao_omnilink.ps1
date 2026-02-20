$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Omnilink $date"
$filename = "..\..\LogTracking\IntegracaoOmnilink-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait