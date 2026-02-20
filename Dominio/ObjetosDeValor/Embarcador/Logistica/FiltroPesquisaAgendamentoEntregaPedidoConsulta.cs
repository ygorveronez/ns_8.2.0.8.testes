using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAgendamentoEntregaPedidoConsulta
    {
        public int CodigoTransportador { get; set; }

        public DateTime? DataAgendamentoInicial { get; set; }

        public DateTime? DataAgendamentoFinal { get; set; }

        public int NotaFiscal { get; set; }

        public int CTe { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public SituacaoAgendamentoEntregaPedido? Situacao { get; set; }

        public double CpfCnpjCliente { get; set; }

        public int CodigoDestino { get; set; }
        
        public string Estado { get; set; }
    }
}
