namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaReforma
    {
        public int CodigoFilial { get; set; }
        public int CodigoTransportador { get; set; }
        public double CpfCnpjFornecedor { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public System.DateTime? DataLimite { get; set; }
        public int Numero { get; set; }
        public Enumeradores.SituacaoReformaPallet Situacao { get; set; }
    }
}
