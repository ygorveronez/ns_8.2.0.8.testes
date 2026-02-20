namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FreteRota
    {
        public Enumeradores.SituacaoRetornoFreteRota situacao { get; set; }
        public dynamic rotaSemFrete { get; set; }

        public dynamic tabelas { get; set; }

        public string Codigo { get; set; }

        public string DescricaoDestinos { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPedagio { get; set; }

        public string Tabela { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string TipoCarga { get; set; }

        public string ModeloVeicularCarga { get; set; }

        public decimal ValorFreteOperador { get; set; }

        public decimal ValorFreteLeilao { get; set; }

        public decimal ValorFreteAPagar { get; set; }

        public decimal ValorFreteTabelaFrete { get; set; }
    }
}
