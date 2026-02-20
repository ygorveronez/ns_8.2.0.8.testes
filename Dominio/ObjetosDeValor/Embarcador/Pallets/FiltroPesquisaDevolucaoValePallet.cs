namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaDevolucaoValePallet
    {
        public int CodigoFilial { get; set; }
        public double CpfCnpjCliente { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public System.DateTime? DataLimite { get; set; }
        public int Nfe { get; set; }
        public int Numero { get; set; }
        public Enumeradores.SituacaoValePallet? Situacao { get; set; }
    }
}
