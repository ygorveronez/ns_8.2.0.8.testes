using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.PedidosVendas
{
    public class FiltroPesquisaPedidoVenda
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataEntregaInicial { get; set; }
        public DateTime DataEntregaFinal { get; set; }
        public StatusPedidoVenda? StatusPedidoVenda { get; set; }
        public TipoPedidoVenda TipoPedidoVenda { get; set; }
        public double CnpjCpfCliente { get; set; }
        public int CodigoFuncionario { get; set; }
        public int CodigoPet { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEmpresa { get; set; }
        public int NumeroInternoInicial { get; set; }
        public int NumeroInternoFinal { get; set; }
    }
}
