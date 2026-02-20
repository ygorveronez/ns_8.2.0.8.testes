$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Evo Solutions $date"
$filename = "..\..\LogTracking\IntegracaoEvoSolutions-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait