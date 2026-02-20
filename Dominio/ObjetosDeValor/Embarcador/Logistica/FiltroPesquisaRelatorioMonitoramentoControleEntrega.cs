using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRelatorioMonitoramentoControleEntrega
    {
        public DateTime DataMonitoramentoInicial { get; set; }
        public DateTime DataMonitoramentoFinal { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string NumeroPedido { get; set; }
        public List<double> Recebedores { get; set; }
        public List<int> Filiais { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }
    }
}
