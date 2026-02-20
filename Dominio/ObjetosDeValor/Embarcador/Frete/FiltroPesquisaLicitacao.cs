namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaLicitacao
    {
        public int CodigoTabelaFrete { get; set; }

        public int CodigoTransportador { get; set; }

        public string Descricao { get; set; }

        public int Numero { get; set; }
    }
}
