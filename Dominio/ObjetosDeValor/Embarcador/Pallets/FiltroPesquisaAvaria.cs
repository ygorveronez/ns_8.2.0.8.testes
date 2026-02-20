namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaAvaria
    {
        public int CodigoFilial { get; set; }
        public int CodigoMotivoAvaria { get; set; }
        public int CodigoSetor { get; set; }
        public int CodigoTransportador { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public System.DateTime? DataLimite { get; set; }
        public int Numero { get; set; }
        public Enumeradores.SituacaoAvariaPallet Situacao { get; set; }
    }
}
