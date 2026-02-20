using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{

    public enum tipoFiltroAtendimento
    {
        Nenhum = 0,
        Pendente = 1,
        Tratativa = 2,
        Atrasada = 3,
        Concluida = 4
    }

    public enum tipoFiltroTendenciaEntrega
    {
        Todos = 99,
        Nenhum = 0,
        Adiantado = 1,
        NoPrazo = 2,
        TendenciaAtraso = 3,
        Atrasado = 4
    }

    public enum tipoFiltroFarolEspelhamento
    {
        Offline = 0,
        Online = 1
    }

    public class FiltroPesquisaAcompanhamentoCarga
    {
        #region Propriedades

        public string NumeroCarga { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public DateTime? DataInicioViagemInicial { get; set; }
        public DateTime? DataInicioViagemFinal { get; set; }
        public List<int> NumeroNotasFiscais { get; set; }
        public List<StatusViagemControleEntrega> StatusViagem { get; set; }
        public DateTime? DataCriacaoCargaInicial { get; set; }
        public DateTime? DataCriacaoCargaFinal { get; set; }
        public DateTime? DataPrevisaoInicioViagemInicial { get; set; }
        public DateTime? DataPrevisaoInicioViagemFinal { get; set; }
        public DateTime? DataPrevisaoEntregaInicial { get; set; }
        public DateTime? DataPrevisaoEntregaFinal { get; set; }
        public DateTime? DataEntregaInicial { get; set; }
        public DateTime? DataEntregaFinal { get; set; }
        public DateTime? DataCarregamentoInicial { get; set; }
        public DateTime? DataCarregamentoFinal { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<string> CodigosFilial { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public List<int> CodigosMotorista { get; set; }
        public List<double> LocaisColeta { get; set; }
        public List<double> LocaisEntrega { get; set; }
        public List<int> localidadeColeta { get; set; }
        public List<int> localidadeEntrega { get; set; }
        public List<int> CodigosVendedor { get; set; }
        public List<SituacaoCarga> SituacoesCarga { get; set; }
        public List<double> CpfCnpjDestinatariosPedido { get; set; }
        public List<double> CpfCnpjRemetentesPedido { get; set; }
        public bool ExibirSomenteCargasComVeiculo { get; set; }
        public bool ExibirSomenteCargasComChamadoAberto { get; set; }
        public bool ExibirSomenteCargasComReentrega { get; set; }
        public bool ExibirSomenteCargasUsuarioMonitora { get; set; }
        public bool ExibirSomenteCargasMotoristaMobile { get; set; }
        public bool ExibirSomenteCargasEmAtraso { get; set; }
        public bool ExibirSomenteCargasCriticas { get; set; }
        public bool ExibirSomenteCargasAlertaMonitoramentoAberto { get; set; }
        public bool ExibirSomenteCargasAlertaMonitoramentoEmTratativa { get; set; }
        public bool ExibirSomenteCargasSemAlertas { get; set; }
        public bool ExibirSomenteCargasComPesquisaDeDesembarquePendente { get; set; }
        public bool? PreTrip { get; set; }
        public bool resumoEmViagem { get; set; }
        public bool resumoNaoIniciada { get; set; }
        public bool resumoTodas { get; set; }
        public bool resumoFinalizadas { get; set; }
        public List<double> CodigosExpedidor { get; set; }
        public List<double> CodigosRecebedor { get; set; }
        public List<double> CpfCnpjRecebedoresOuSemRecebedores { get; set; }
        public List<int> CodigosFilialVenda { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoNovaCarga { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosStatusDaViagem { get; set; }
        public List<int> CodigosTipoDocumentoTransporte { get; set; }
        public tipoFiltroAtendimento FiltroAtendimento { get; set; }
        public string NumeroOrdem { get; set; }
        public bool? PossuiRecebedor { get; set; }
        public bool? PossuiExpedidor { get; set; }
        public tipoFiltroTendenciaEntrega FiltroTendenciaEntrega { get; set; }
        public tipoFiltroTendenciaEntrega FiltroTendenciaColeta { get; set; }
        public tipoFiltroTendenciaEntrega FiltroTendencia { get; set; }

        public bool ExibirEntregaAntesEtapaTransporte { get; set; }

        public DateTime DataAgendamentoPedidoInicial { get; set; }

        public DateTime DataAgendamentoPedidoFinal { get; set; }

        public DateTime DataColetaPedidoInicial { get; set; }

        public DateTime DataColetaPedidoFinal { get; set; }

        public List<double> CodigosClienteComplementar { get; set; }

        public string NumeroPedidoCliente { get; set; }
        public int CanalVenda { get; set; }
        public bool VeiculoNoRaio { get; set; }
        public OpcoesOrdenacaoCardsAcompanhamentoCarga PropriedadeOrdenacaoCargasAcompanhamentoCarga { get; set; }
        public TipoCobrancaMultimodal ModalTransporte { get; set; }
        public List<MonitoramentoStatus> MonitoramentoStatus { get; set; }
        public List<int> TipoAlerta { get; set; }
        public string EscritorioVenda { get; set; }

        public string EquipeVendas { get; set; }

        public string TipoMercadoria { get; set; }
        public string RotaFrete { get; set; }
        public List<int> Mesoregiao { get; set; }
        public List<int> Regiao { get; set; }
        public List<int> GrupoDePessoas { get; set; }
        public bool? Parqueada { get; set; }

        public string Matriz { get; set; }
        public int CanalEntrega { get; set; }
        public bool? ExibirSomenteCargasFarolEspelhamentoOnline { get; set; }
        public bool? ExibirSomenteCargasFarolEspelhamentoOffline { get; set; }
        public DateTime DataInicioAbate { get; set; }
        public DateTime DataFimAbate { get; set; }
        #endregion
    }
}
