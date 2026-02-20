using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaEntrega
    {
        #region Propriedades

        public bool VisualizarAtendimentosOperadorResponsavel { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string CodigoSap { get; set; }

        public List<int> CodigosMotorista { get; set; }
        public List<int> CodigosCarga { get; set; }

        public int CodigoResponsavelEntrega { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosTransportador { get; set; }
        public int CodigoTransportador { get; set; }

        public List<int> CodigosVeiculo { get; set; }

        public List<int> CodigosVendedor { get; set; }
        public List<int> CodigosStatusViagem { get; set; }
        public List<int> CodigosTipoOcorrencia { get; set; }

        public List<int> CodigosSupervisor { get; set; }

        public List<int> CodigosGerente { get; set; }

        public List<double> CpfCnpjDestinatarios { get; set; }

        public List<double> CpfCnpjRemetentes { get; set; }
        public double CnpjRemetente { get; set; }
        public double CnpjDestinatario { get; set; }

        public List<double> CpfCnpjEmitentes { get; set; }

        public List<double> CpfCnpjExpedidores { get; set; }
        public double CnpjTomador { get; set; }

        public DateTime DataInicial { get; set; }
        public DateTime DataInicioEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }

        public DateTime DataLimite { get; set; }
        public DateTime DataProgramadaColetaFinal { get; set; }
        public DateTime DataProgramadaColetaInicial { get; set; }
        public DateTime DataCarregamentoCargaFinal { get; set; }
        public DateTime DataCarregamentoCargaInicial { get; set; }
        public DateTime DataInicioAbate { get; set; }
        public DateTime DataFimAbate { get; set; }
        public bool ExibirEntregaAntesEtapaTransporte { get; set; }

        public bool ExibirSomenteCargasComVeiculo { get; set; }

        public bool ExibirSomenteCargasSubTrecho { get; set; }

        public int NumeroPedido { get; set; }

        public List<string> NumeroPedidosEmbarcador { get; set; }

        public string Placa { get; set; }

        public Enumeradores.SituacaoResponsavelEntrega SituacaoResponsavel { get; set; }

        public Enumeradores.StatusViagemControleEntrega? StatusViagem { get; set; }

        public List<int> NumeroNotasFiscais { get; set; }

        public bool ExibirSomenteCargasComChamadoAberto { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public bool ExibirSomenteCargasComChatNaoLido { get; set; }

        public bool ExibirSomenteCargasComReentrega { get; set; }

        public bool ExibirSomenteCargasComMotoristaAppDesatualizado { get; set; }
        public bool SomenteCargaComEstadiaConfiguraca { get; set; }

        public DateTime DataEntregaPedidoInicial { get; set; }

        public DateTime DataAgendamentoInicial { get; set; }
        public DateTime DataAgendamentoFinal { get; set; }

        public DateTime DataEntregaPedidoFinal { get; set; }

        public DateTime DataPrevisaoEntregaPedidoInicial { get; set; }

        public DateTime DataPrevisaoEntregaPedidoFinal { get; set; }

        public DateTime DataPrevisaoInicioViagemInicial { get; set; }

        public DateTime DataPrevisaoInicioViagemFinal { get; set; }

        public string SerieNota { get; set; }
        public DateTime DataEmissaoNotaDe { get; set; }
        public DateTime DataEmissaoNotaAte { get; set; }
        public int NumeroCTeDe { get; set; }
        public int NumeroCTeAte { get; set; }
        public int SerieCTe { get; set; }
        public DateTime DataEmissaoCTeDe { get; set; }
        public DateTime DataEmissaoCTeAte { get; set; }
        public int EmpresaDestino { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public int CidadeOrigem { get; set; }
        public int CidadeDestino { get; set; }
        public List<string> EstadosOrigem { get; set; }
        public List<string> EstadosDestino { get; set; }
        public string NumeroSolicitacao { get; set; }
        public string NumeroPedidoCF { get; set; }
        public string NumeroOrdemPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int GrupoPessoa { get; set; }
        public int TipoOperacao { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<string> CodigoCargaEmbarcadorMulti { get; set; }
        public List<double> Recebedor { get; set; }
        public bool? CargasComEstadiasGeradas { get; set; }
        public bool? ExibirSomenteCargasComRecebedor { get; set; }
        public bool? ExibirSomenteCargasComExpedidor { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public bool RetornarInformacoesMonitoramento { get; set; }
        public bool RetornarCargasQueMonitoro { get; set; }
        public DateTime DataProgramadaDescargaInicial { get; set; }
        public DateTime DataProgramadaDescargaFinal { get; set; }
        public string DescSituacaoEntrega { get; set; }
        public List<int> CanaisEntrega { get; set; }
        public List<int> CanaisVenda { get; set; }
        public List<Enumeradores.StatusViagemControleEntrega> StatusViagens { get; set; }
        public List<int> CentrosCarregamentos { get; set; }
        public List<int> CentrosResultados { get; set; }
        public int CodigoResponsavelVeiculo { get; set; }
        public bool OrdernarResultadosPorDataCriacao { get; set; }
        public bool SomenteCargasCriticas { get; set; }
        public bool SomenteCargasComPesquisaRecebedorPendenteResposta { get; set; }
        public bool PermiteExibirCargaCancelada { get; set; }
        public DateTime DataInicioViagemInicial { get; set; }
        public DateTime DataInicioViagemFinal { get; set; }
        public List<int> TiposTrecho { get; set; }

        public DateTime DataInicioCriacaoCarga { get; set; }
        public DateTime DataFinalCriacaoCarga { get; set; }
        public List<int> CodigosOrigem { get; set; }
        public List<int> CodigosTiposOperacao { get; set; }
        public int Filial { get; set; }
        public List<SituacaoCarga> SituacoesCarga { get; set; }
        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }
        public bool? RastreadorOnlineOffline { get; set; }
        public TendenciaEntrega TendenciaEntrega { get; set; }
        public TendenciaEntrega TendenciaColeta { get; set; }

        public List<double> ClienteComplementar { get; set; }

        public bool VeiculosNoRaio { get; set; }

        public TipoCobrancaMultimodal ModalTransporte { get; set; }
        public List<MonitoramentoStatus> MonitoramentoStatus { get; set; }
        public List<int> TipoAlerta { get; set; }
        public string EscritorioVenda { get; set; }

        public string EquipeVendas { get; set; }

        public string TipoMercadoria { get; set; }
        public string RotaFrete { get; set; }
        public List<int> MesoRegiao { get; set; }
        public List<int> Regiao { get; set; }
        public string Matriz { get; set; }
        public bool? Parqueada { get; set; }
        public bool? FiltrarPorProcessamentoEmLote { get; set; }

        public bool PermitirBuscarCargasAgrupadasAoPesquisarNumero { get; set; }

        #endregion

        #region Propriedades com Regras

        public int CodigoFilial
        {
            set
            {
                if (value > 0)
                    CodigosFilial = new List<int>() { value };
            }
        }

        public int CodigoTipoOperacao
        {
            set
            {
                if (value > 0)
                    CodigosTipoOperacao = new List<int>() { value };
            }
        }

        #endregion
    }
}
