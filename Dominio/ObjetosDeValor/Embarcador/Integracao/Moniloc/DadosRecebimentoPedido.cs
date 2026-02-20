using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc
{
    public class DadosRecebimentoPedidoProduto
    {
        public string CodigoProduto { get; set; }
        public decimal QuantidadeSolicitada { get; set; }
        public decimal QuantidadeColetada { get; set; }
        public DateTime DataEntrega { get; set; }
        public DateTime DataColeta { get; set; }
        public string NumeroRelease { get; set; }
        public string Embalagem { get; set; }
        public decimal AlturaEmbalagem { get; set; }
        public decimal LarguraEmbalagem { get; set; }
        public decimal ComprimentoEmbalagem { get; set; }
        public decimal PesoEmbalagem { get; set; }
        public decimal QuantidadeProdutoEmbalagem { get; set; }
        public decimal PesoProduto { get; set; }
        public int EmpilhamentoMaximo { get; set; }
    }

    public class DadosRecebimentoPedidoDeposito
    {
        public string CodigoDeposito { get; set; }
        public string NomeDeposito { get; set; }
    }

    public class DadosRecebimentoPedidoColeta
    {
        public string HoraColeta { get; set; }
        public decimal QuantidadeConfirmada { get; set; }
        public string Destino { get; set; }
        public string TipoVeiculo { get; set; }
    }
}
