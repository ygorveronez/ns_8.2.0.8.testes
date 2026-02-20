using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class FiltroPesquisaRotaControleEntrega
    {
        #region Propriedades
        public string CodigoCargaEmbarcador { get; set; }
        public List<int> NumerosPedido { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public List<double> CpfCnpjDestinatarios { get; set; }
        public List<double> Recebedores { get; set; }
        public List<double> CpfCnpjEmitentes { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> NumeroNotasFiscais { get; set; }
        public DateTime DataEntregaPedidoInicial { get; set; }
        public DateTime DataEntregaPedidoFinal { get; set; }
        public DateTime DataPrevisaoEntregaPedidoInicial { get; set; }
        public DateTime DataPrevisaoEntregaPedidoFinal { get; set; }

        #endregion
    }
}