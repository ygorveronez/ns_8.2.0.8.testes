using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAgendamentoEntrega
    {
        public string Carga { get; set; }

        public string NumeroOrdem { get; set; }

        public List<double> CodigosClientes { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public List<int> CodigosTransportadores { get; set; }

        public DateTime? DataAgendamentoInicial { get; set; }

        public DateTime? DataAgendamentoFinal { get; set; }

        public DateTime? DataCarregamentoInicial { get; set; }

        public DateTime? DataCarregamentoFinal { get; set; }

        public DateTime? DataPrevisaoEntregaInicial { get; set; }

        public DateTime? DataPrevisaoEntregaFinal { get; set; }

        public int NFe { get; set; }
        
        public List<SituacaoAgendamentoEntregaPedido> SituacoesAgendamento { get; set; }
        
        public List<int> CodigosSituacaoNotaFiscal { get; set; }

        public DateTime? DataCriacaoPedidoInicial { get; set; }

        public DateTime? DataCriacaoPedidoFinal { get; set; }

        public bool? PossuiDataTerminoCarregamento { get; set; }

        public bool? PossuiDataSugestaoEntrega { get; set; }
        
        public SimNao? PossuiNotaFiscalVinculada { get; set; }

        public DateTime? DataInicialSugestaoEntrega { get; set; }

        public DateTime? DataFinalSugestaoEntrega { get; set; }
        
        public bool SomenteCargasFinalizadas { get; set; }

        public List<int> CanalEntrega { get; set; }

        public bool? ObrigaCarga { get; set; }

        public DateTime? DataInicialCriacaoDaCarga { get; set; }

        public DateTime? DataFinalCriacaoDaCarga { get; set; }

        public string SenhaEntregaAgendamento { get; set; }

        public bool? EntegasComSenhaDeAgendamento { get; set; }

        public List<string> SiglasUFDestino { get; set; }
    }
}
