using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaMonitoramento
    {
        public DateTime DataInicioMonitoramento { get; set; }
        public DateTime DataFimMonitoramento { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string DescricaoAlerta { get; set; }
        public bool? RastreadorOnlineOffline { get; set; }

        public List<MonitoramentoStatus> Status { get; set; }

        public TipoAlertaCarga TipoAlertaCarga { get; set; }
        public int GrupoStatusViagem { get; set; }

        public TipoAlerta TipoAlerta { get; set; }

        public List<int> CodigosStatusViagem { get; set; }

        public List<int> CodigosGrupoTipoOperacao { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public int CodigoGrupoPessoa { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public string NumeroPedido { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public DateTime DataEntregaPedidoInicio { get; set; }

        public DateTime DataEntregaPedidoFinal { get; set; }

        public DateTime PrevisaoEntregaInicio { get; set; }

        public DateTime PrevisaoEntregaFinal { get; set; }

        public bool? PossuiExpedidor { get; set; }

        public bool? PossuiRecebedor { get; set; }

        public List<double> Destinatario { get; set; }

        public List<double> Recebedores { get; set; }

        public List<int> CodigoCargaEmbarcadorMulti { get; set; }

        public DateTime DataEmissaoNFeInicio { get; set; }

        public DateTime DataEmissaoNFeFim { get; set; }

        public bool SomenteRastreados { get; set; }

        public bool SomenteUltimoPorCarga { get; set; }

        public MonitoramentoFiltroCliente FiltroCliente { get; set; }

        public double Cliente { get; set; }

        public int CodigoCategoriaPessoa { get; set; }

        public int CodigoFuncionarioVendedor { get; set; }

        public string NumeroEXP { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosVeiculos { get; set; }

        public List<long> CodigosExpedidores { get; set; }

        public bool VeiculosComContratoDeFrete { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public List<int> CodigosCarga { get; set; }

        public List<int> CodigosDestinos { get; set; }

        public List<double> CodigoClienteDestino { get; set; }

        public List<double> CodigoClienteOrigem { get; set; }

        public List<int> CodigosOrigem { get; set; }

        public List<string> EstadosOrigem { get; set; }

        public List<string> EstadosDestino { get; set; }

        public List<int> CodigosResponsavelVeiculo { get; set; }

        public List<int> CodigosCentroResultado { get; set; }

        public List<double> CodigosFronteiraRotaFrete { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public List<int> CodigosPaisDestino { get; set; }

        public List<int> CodigosPaisOrigem { get; set; }

        public bool ApenasMonitoramentosCriticos { get; set; }

        public bool VeiculosEmLocaisTracking { get; set; }

        public List<int> LocaisTracking { get; set; }

        public List<int> CodigosTiposTrecho { get; set; }

        public List<Dominio.Entidades.Embarcador.Logistica.Locais> locais { get; set; }

        public List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> RaiosProximidade { get; set; }

        public DateTime DataInicioCarregamento { get; set; }

        public DateTime DataFimCarregamento { get; set; }

        public DateTime InicioViagemPrevistaInicial { get; set; }

        public DateTime InicioViagemPrevistaFinal { get; set; }

        public int CodigoMotorista { get; set; }

        public List<double> Remetente { get; set; }

        public List<int> TiposCarga { get; set; }

        public List<int> Produtos { get; set; }

        public DateTime DataRealEntrega { get; set; }

        public List<int> LocaisRaioProximidade { get; set; }

        public bool? MostrarRaiosProximidade { get; set; }

        public DateTime DataAgendamentoPedidoInicial { get; set; }

        public DateTime DataAgendamentoPedidoFinal { get; set; }

        public DateTime DataColetaPedidoInicial { get; set; }

        public DateTime DataColetaPedidoFinal { get; set; }

        public List<double> CodigosClienteComplementar { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public int CanalVenda { get; set; }

        public SituacaoIntegracao? SituacaoIntegracaoSM { get; set; }

        public bool VeiculoNoRaio { get; set; }

        public TipoCobrancaMultimodal ModalTransporte { get; set; }

        public int? CodigoEvento { get; set; }

        public string EscritorioVenda { get; set; }

        public string EquipeVendas { get; set; }

        public string TipoMercadoria { get; set; }

        public string RotaFrete { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega DataBaseCalculoPrevisaoControleEntrega { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento TelaMonitoramentoApresentarCargasQuando { get; set; }
        public int MonitoramentoStatusViagemTipoRegra { get; set; }

        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }

        public bool TelaMonitoramentoFiltroFilialDaCarga { get; set; }

        public List<int> Mesoregiao { get; set; }

        public List<int> Regiao { get; set; }
        public int SituacaoCarga { get; set; }
        public string Matriz { get; set; }
        public int TendenciaEntrega { get; set; }
        public int? GrupoTipoOperacaoIndicador { get; set; }
        public bool? ComAlerta { get; set; }

        public bool? Parqueada { get; set; }

        public List<int> Vendedor { get; set; }

        public List<int> Supervisor { get; set; }
        public bool? ColetaNoPrazo { get; set; }
        public bool? EntregaNoPrazo { get; set; }
        public List<TendenciaEntrega> TendenciaProximaColeta { get; set; }
        public List<TendenciaEntrega> TendenciaProximaEntrega { get; set; }

        public List<int> TipoAlertaEvento { get; set; }

    }
}
