function DownloadDacte(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadDacte", { CodigoCTe: cte.data.Codigo });
}

function DownloadXML(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXML", { CodigoCTe: cte.data.Codigo });
}