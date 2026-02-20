using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioParadas
    {
        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataEntregaInicial { get; set; }

        public DateTime? DataEntregaFinal { get; set; }

        public List<string> NumeroCargas { get; set; }

        public List<int> CodigosTipoCargas { get; set; }

        public List<int> CodigosTipoOperacoes { get; set; }

        public List<int> CodigosTransportadores { get; set; }

        public List<int> CodigosFiliais { get; set; }

        public List<int> CodigosVeiculos { get; set; }

        public List<int> CodigosMotoristas { get; set; }

        public List<int> CodigosGrupoPessoas { get; set; }

        public List<double> CpfsCnpjsRemetentes { get; set; }

        public List<double> CpfsCnpjsDestinatarios { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoDestino { get; set; }
        public string CodigoIntegracaoCliente { get; set; }
        public string NumeroCarga { get; set; }
        public string ProtocoloIntegracaoSM { get; set; }
        public string EscritorioVendas { get; set; }
        public string NumeroPedidoCliente { get; set; }

        public bool? TipoParada { get; set; }
        public bool? Transbordo { get; set; }

        public DateTime? DataEntregaPlanejadaInicio { get; set; }

        public DateTime? DataEntregaPlanejadaFinal { get; set; }

        public List<MonitoramentoStatus> MonitoramentoStatus { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }
        public bool ExibirCargasAgrupadas { get; set; }
    }
}
