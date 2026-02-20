$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração BrasilRiskPosicoes $date"
$filename = "..\..\LogTracking\IntegracaoBrasilRiskPosicoes-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait