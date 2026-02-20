namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaTransferenciaPallet
    {
        public int CodigoFilial { get; set; }
        public int CodigoSetor { get; set; }
        public int CodigoTurno { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public System.DateTime? DataLimite { get; set; }
        public int Numero { get; set; }
        public Enumeradores.SituacaoTransferenciaPallet Situacao { get; set; }
    }
}
