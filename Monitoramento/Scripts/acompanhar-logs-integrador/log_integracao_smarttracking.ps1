$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração SmartTracking $date"
$filename = "..\..\LogTracking\IntegracaoSmartTracking-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait