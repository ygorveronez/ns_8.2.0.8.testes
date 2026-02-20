$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Enviar Notificacoes Alertas $date"
$filename = "..\..\LogTracking\EnviarNotificacoesAlertas-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait