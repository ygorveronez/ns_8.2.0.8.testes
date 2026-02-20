using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class GiroEstoque
    {
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string GrupoProduto { get; set; }
        public UnidadeDeMedida UnidadeMedida { get; set; }
        public decimal QuantidadeEntrada { get; set; }
        public decimal QuantidadeSaida { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal Giro { get; set; }
        public decimal ValorEntrada { get; set; }
        public decimal ValorSaida { get; set; }
        public decimal TotalEntrada { get; set; }
        public decimal TotalSaida { get; set; }
        public string Empresa { get; set; }
        private decimal MediaMes { get; set; }
        private decimal MediaAno { get; set; }

        private decimal EstoqueReservado { get; set; }

        public string EstoqueReservadoFormatado
        {
            get { return EstoqueReservado.ToString("n4"); }
        }

        public string EstoqueAtualFormatado
        {
            get { return EstoqueAtual.ToString("n4"); }
        }

        public string GiroFormatado
        {
            get { return Giro.ToString("n4"); }
        }

        public string UnidadeMedidaFormatada
        {
            get { return UnidadeMedida.ObterSigla(); }
        }

        public string MediaMesFormatado
        {
            get { return MediaMes.ToString("n2"); }
        }

        public string MediaAnoFormatado
        {
            get { return MediaAno.ToString("n2"); }
        }
    }
}