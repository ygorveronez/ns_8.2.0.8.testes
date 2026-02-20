namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class CTeOracle
    {
        public CTeOracle()
        {
            this.Info = new Resultado();
        }
        public string DataRecibo;
        public string CodStatusEnvio;
        public string DescricaoEnvio;
        public string NumeroRecibo;
        public string DataProtocolo;
        public string CodStatusProtocolo;
        public string DescricaoProtocolo;
        public string NumeroProtocolo;
        public string ChaveCTE;
        public string DigVerificador;
        public string StatusIntegrador;
        public string DescricaoStatusIntegrador;
        public int CodigoCTeInterno;
        public int CodigoInutilizacao;
        public string PDFDacte;
        public string XML;
        public string XMLCancelamento;
        public Resultado Info;
    }
}
