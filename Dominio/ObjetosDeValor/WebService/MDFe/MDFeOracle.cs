namespace Dominio.ObjetosDeValor.WebService.MDFe
{
    public class MDFeOracle
    {
        public MDFeOracle()
        {
            this.Info = new CTe.Resultado();
        }
        public string DataRecibo;
        public string CodStatusEnvio;
        public string DescricaoEnvio;
        public string NumeroRecibo;
        public string DataProtocolo;
        public string CodStatusProtocolo;
        public string DescricaoProtocolo;
        public string NumeroProtocolo;
        public string ChaveMDFe;
        public string DigVerificador;
        public string StatusIntegrador;
        public string DescricaoStatusIntegrador;
        public int CodigoMDFeAutorizacao;
        public int CodigoMDFeEncerramento;
        public int CodigoMDFeCancelamento;
        public int CodigoMDFeEvento;
        public string PDFDAMDFE;
        public string XMLAutorizacao;
        public string XMLCancelamento;
        public string XMLEncerramento;
        public string XMLEvento;
        public CTe.Resultado Info;
    }
}
