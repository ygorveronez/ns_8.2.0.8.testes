$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Onix Sat $date"
$filename = "..\..\LogTracking\IntegracaoOnixSat-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait