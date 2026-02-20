$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Autotrac $date"
$filename = "..\..\LogTracking\IntegracaoAutotrac-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait