namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaDevolucaoValePalletAprovacao
    {
        public int Codigo { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoSetor { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoUsuario { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public System.DateTime? DataLimite { get; set; }
        public Enumeradores.SituacaoDevolucaoValePallet Situacao { get; set; }
    }
}
