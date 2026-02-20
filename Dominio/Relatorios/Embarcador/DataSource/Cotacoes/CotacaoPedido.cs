using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cotacoes
{
    public class CotacaoPedido
    {
        public long Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataPrevista { get; set; }
        public string Usuario { get; set; }
        public string Pessoa { get; set; }
        public string TipoOperacao { get; set; }
        public string Solicitante { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Modal { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public int Volumes { get; set; }
        public int Pallets { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoCubo { get; set; }
        public decimal ValorMercadoria { get; set; }
        public int QuantidadeNotaFiscal { get; set; }
        public int QuantidadeEntrega { get; set; }
        public int QuantidadeAjudante { get; set; }
        public int QuantidadeEscoltaArmada { get; set; }
        public string ModeloVeiculo { get; set; }
        public DateTime DataInicialColeta { get; set; }
        public DateTime DataFinalColeta { get; set; }
        public string Observacao { get; set; }

        public decimal ValorCotacao { get; set; }
        public decimal PercentualAcrescimo { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorTotalCotacao { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorTotalComICMS { get; set; }

        //Detalhes
        public string Componente { get; set; }
        public decimal ComponenteValor { get; set; }

        public string DataInicialColetaFormatada
        {
            get { return DataInicialColeta == DateTime.MinValue ? "" : DataInicialColeta.ToString("dd/MM/yyyy"); }
        }

        public string DataFinalColetaFormatada
        {
            get { return DataFinalColeta == DateTime.MinValue ? "" : DataFinalColeta.ToString("dd/MM/yyyy"); }
        }
    }
}
