using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido
{
    public sealed class AndamentoPedido
    {
        public string NumeroPedido { get; set; }

        public string CodigoCliente { get; set; }

        public string NomeCliente { get; set; }

        public string CNPJCliente { get; set; }

        public string CodigoCentro { get; set; }

        public string DescricaoCentro { get; set; }

        public string TipoCarregamento { get; set; }

        public string DescricaoTipoCarregamento { get; set; }

        public List<FluxosStatusPedido> FluxoPedido { get; set; }

        public List<DadosQuantidadesPendente> ItensPendentes { get; set; }

        public List<Remessa> Remessas { get; set; }

        public List<LogsErro> LogsErros { get; set; }
    }

    public sealed class Remessa
    {
        public string NumeroRemessa { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public string DataEmissao { get; set; }

        public List<FluxoRemessa> FluxoRemessa { get; set; }

        public List<RemessasVinculadasPedidoPesquisado> DetalhesRemessa { get; set; }
    }
}
