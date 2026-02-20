namespace Dominio.ObjetosDeValor.NFSeENotas
{
    public class NFSe
    {
        public string numero { get; set; }
        public string dataAutorizacao { get; set; }
        public string dataCriacao { get; set; }
        public string dataUltimaAlteracao { get; set; }
        public string chaveAcesso { get; set; }
        public string codigoVerificacao { get; set; }
        public string linkDownloadPDF { get; set; }
        public string linkDownloadXML { get; set; }
        public string status { get; set; }
        public string motivoStatus { get; set; }
        public bool enviadaPorEmail { get; set; }
        public Cliente cliente { get; set; }
        public string id { get; set; }
        public string ambienteEmissao { get; set; }
        public string tipo { get; set; }
        public string idExterno { get; set; }
        public string consumidorFinal { get; set; }
        public string indicadorPresencaConsumidor { get; set; }
        public Servico servico { get; set; }
        public decimal valorTotal { get; set; }
        public string idExternoSubstituir { get; set; }
        public string nfeIdSubstitituir { get; set; }
    }
}
