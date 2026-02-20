$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Sighra $date"
$filename = "..\..\LogTracking\IntegracaoSighra-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait