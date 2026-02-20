using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.PedidosVendas
{
    public class FiltroPesquisaRelatorioPedidoVenda
    {
        public int CodigoEmpresa { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int CodigoVendedor { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoServico { get; set; }
        public double CnpjFornecedorServico { get; set; }
        public int CodigoFuncionarioServico { get; set; }
        public double CnpjPessoa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataEntregaInicial { get; set; }
        public DateTime DataEntregaFinal { get; set; }
        public StatusPedidoVenda Status { get; set; }
        public TipoPedidoVenda Tipo { get; set; }
        public bool ExibirItens { get; set; }
        public int NumeroInternoInicial { get; set; }
        public int NumeroInternoFinal { get; set; }
    }
}
