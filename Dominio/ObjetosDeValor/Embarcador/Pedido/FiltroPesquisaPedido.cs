using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaPedido
    {
        #region Propriedades

        public List<int> CodigosCidadePoloOrigem { get; set; }

        public List<int> CodigosCidadePoloDestino { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoPedidoViagemNavio { get; set; }

        public int Container { get; set; }

        public int CodigoPedidoMinimo { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosPedido { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosVendedor { get; set; }

        public List<int> CodigosSupervisor { get; set; }

        public List<int> CodigosGerente { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosTipoOperacaoDiferenteDe { get; set; }

        public List<string> NumeroControlesPedido { get; set; }

        public DateTime? PrevisaoDataInicial { get; set; }

        public DateTime? PrevisaoDataFinal { get; set; }

        public DateTime? DataColeta { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public DateTime? DataCriacaoPedidoInicio { get; set; }

        public DateTime? DataCriacaoPedidoLimite { get; set; }

        public List<double> Destinatarios { get; set; }

        public List<int> CodigosDestino { get; set; }

        public bool DisponivelColeta { get; set; }

        public List<string> EstadosDestino { get; set; }

        public List<string> EstadosOrigem { get; set; }

        public bool ExibirPedidosExpedidor { get; set; }

        public Enumeradores.TipoPessoa? TipoPessoa { get; set; }

        public bool FiltrarPorParteDoNumero { get; set; }

        public bool CompartilharAcessoEntreGrupoPessoas { get; set; }

        public bool PedidosAberto { get; set; }

        public List<int> CodigosGrupoPessoa { get; set; }

        public List<int> CodigosGrupoPessoaRetirada { get; set; }

        public List<int> CodigosMotorista { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroCarregamento { get; set; }

        public string DescricaoFiltro { get; set; }

        public int NumeroNotaFiscal { set { NumeroNotasFiscais = value > 0 ? new List<int>() { value } : null; } }

        public List<int> NumeroNotasFiscais { get; set; }

        public string NumeroOS { get; set; }

        public int NumeroPedido { get; set; }

        public int CodigoPedido { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public string NumeroPedidoEmbarcadorDe { get; set; }

        public string NumeroPedidoEmbarcadorAte { get; set; }

        public string Ordem { get; set; }

        public bool OrdernarPorPrioridade { get; set; }

        public List<int> CodigosOrigem { get; set; }

        public List<int> CodigosPaisDestino { get; set; }

        public List<int> CodigosPaisOrigem { get; set; }

        public int PedidoCentroCusto { get; set; }

        public int PedidoEmpresaResponsavel { get; set; }

        public bool OcultarPedidosProvisorios { get; set; }

        public bool PedidosRedespacho { get; set; }

        public bool PedidosOrigemRecebedor { get; set; }

        public List<int> PedidosSelecionados { get; set; }

        public bool PedidoSemCarga { get; set; }

        public bool OcultarPedidosRetiradaProdutos { get; set; }

        public bool PedidoSemCargaPedido { get; set; }

        public bool PedidoParaReentrega { get; set; }

        public int PedidosSessao { get; set; }

        public bool PedidoSemCarregamento { get; set; }

        public string PortoSaida { get; set; }

        public double Expedidor { get; set; }

        public double Recebedor { get; set; }

        public int Rota { get; set; }

        public List<double> Remetentes { get; set; }

        public List<double> CodigosRemetenteDestinatarioRetirada { get; set; }

        public double ClientePortal { get; set; }

        public string Reserva { get; set; }

        public Enumeradores.SituacaoPedido? Situacao { get; set; }

        public Enumeradores.SituacaoAcompanhamentoPedido? SituacaoAcompanhamentoPedido { get; set; }

        public Enumeradores.SituacaoPlanejamentoPedido? SituacaoPlanejamentoPedido { get; set; }

        public List<SituacaoPlanejamentoPedidoTMS> SituacaoPlanejamentoPedidoTMS { get; set; }

        public bool SomenteComReserva { get; set; }

        public string TipoEmbarque { get; set; }

        public int TipoOperacao { get; set; }

        public double ProvedorOS { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public List<double> Tomadores { get; set; }

        public bool FiltarRementeDestinatarioPorTransportador { get; set; }

        public bool FiltrarPedidosCargaFechada { get; set; }

        public int CodigoTransportador { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public bool? Transbordo { get; set; }

        public List<int> CodigosVeiculo { get; set; }

        public int GrupoPessoaDestinatario { get; set; }

        public int CategoriaPessoa { get; set; }

        public bool ProgramaComSessaoRoteirizador { get; set; }

        public Enumeradores.OpcaoSessaoRoteirizador OpcaoSessaoRoteirizador { get; set; }

        public int CodigoSessaoRoteirizador { get; set; }

        public Embarcador.Enumeradores.SituacaoSessaoRoteirizador SituacaoSessaoRoteirizador { get; set; }

        public Enumeradores.TipoFiltroDataMontagemCarga? TipoFiltroData { get; set; }

        public string DeliveryTerm { get; set; }

        public string IdAutorizacao { get; set; }

        public DateTime? DataInclusaoPCPInicial { get; set; }

        public DateTime? DataInclusaoPCPLimite { get; set; }

        public DateTime? DataInclusaoBookingInicial { get; set; }

        public DateTime? DataInclusaoBookingLimite { get; set; }

        public decimal PesoDe { get; set; }

        public decimal PesoAte { get; set; }

        public decimal PalletDe { get; set; }

        public decimal PalletAte { get; set; }

        public decimal VolumeDe { get; set; }

        public decimal VolumeAte { get; set; }

        public decimal ValorDe { get; set; }

        public decimal ValorAte { get; set; }

        public int TipoCarga { get; set; }

        public int TipoDeCarga { get; set; }

        public List<int> TiposDeCargas { get; set; }

        public bool NaoRecebeCargaCompartilhada { get; set; }

        public bool SomentePedidosComNota { get; set; }

        public bool SomentePedidosSemNota { get; set; }

        public int CodigoFuncionarioVendedor { get; set; }

        public string NumeroEXP { get; set; }

        public int Deposito { get; set; }

        public string NumeroTransporte { get; set; }

        public List<int> CodigosCanalEntrega { get; set; }

        public List<int> CodigosLinhaSeparacao { get; set; }

        public List<int> CodigosRestricoesEntrega { get; set; }

        public List<int> CodigosProdutos { get; set; }

        public List<int> CodigosGrupoProdutos { get; set; }

        public List<int> CodigosCategoriaClientes { get; set; }

        public int CodigoAutor { get; set; }

        public bool UsarTipoTomadorPedido { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public List<Dominio.Enumeradores.TipoTomador> TiposTomador { get; set; }

        public List<int> CodigosRotaFrete { get; set; }

        public List<int> CodigosUsuario { get; set; }

        public string UsuarioRemessa { get; set; }

        public int Transportador { get; set; }

        public int TransportadoraMatriz { get; set; }

        public string ProcImportacao { get; set; }

        public bool FiltroRetirada { get; set; }

        public string TipoPropriedadeVeiculo { get; set; }

        public List<PedidosVinculadosCarga> VinculoCarga { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa? AceitaContratarTerceiros { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public int CodigoProcessamentoEspecial { get; set; }

        public int CodigoHorarioEntrega { get; set; }

        public int CodigoZonaTransporte { get; set; }

        public int CodigoDetalheEntrega { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> RestricaoDiasEntrega { get; set; }

        public string NumeroCarregamentoPedido { get; set; }

        public string CodigoAgrupamentoCarregamento { get; set; }

        public Dominio.Enumeradores.OpcaoSimNaoPesquisa? CargaPerigosa { get; set; }

        public int NumeroCotacao { get; set; }

        public int NumeroProtocoloIntegracaoPedido { get; set; }

        public List<int> CodigosCentroResultado { get; set; }

        public List<int> CodigosFuncionarioResponsavel { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public List<double> CodigosFronteiras { get; set; }

        public List<int> CodigosGestores { get; set; }

        public List<int> CodigosModelosVeicularesCarga { get; set; }

        public List<int> CodigosSegmentosVeiculos { get; set; }

        public int CodigoOperador { get; set; }

        public string IdDemanda { get; set; }

        public string Placa { get; set; }

        public bool NaoExibirPedidosDoDia { get; set; }

        public bool NaoMostrarCargasDeslocamentoVazio { get; set; }

        public List<double> ListaRecebedor { get; set; }

        public List<double> ListaExpedidor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumAceiteMotorista? AceiteMotorista { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao? PedidosBloqueados { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao? PedidosRestricaoData { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao? PedidosRestricaoPercentual { get; set; }

        public bool FiltrarPorMultiplosRegistros { get; set; }

        public Enumeradores.TipoControleSaldoPedido? TipoControleSaldoPedido { get; set; }

        public TipoRoteirizacaoColetaEntrega? TipoRoteirizacaoColetaEntrega { get; set; }

        public OrdenacaoFiltroPesquisaPedido? Ordenacao { get; set; }

        public DateTime? DataAgendamentoInicial { get; set; }

        public DateTime? DataAgendamentoFinal { get; set; }

        public bool VisualizarApenasParaPedidoDesteTomador { get; set; }

        public bool VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public string ChaveNotaFiscalEletronica { get; set; }

        public List<int> NotaFiscal { get; set; }

        public string NumeroOrdem { get; set; }

        public int SituacaoComercialPedido { get; set; }

        public bool BloquearSituacaoComercialPedido { get; set; }

        public bool FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador { get; set; }

        public List<CategoriaOS> CategoriaOS { get; set; }

        public List<TipoOSConvertido> TipoOSConvertido { get; set; }

        public bool UsuarioUtilizaSegregacaoPorProvedor { get; set; }

        public List<double> CodigosProvedores { get; set; }

        public List<string> CodigosAgrupadores { get; set; }

        public bool HabilitarCadastroArmazem { get; set; }

        public bool FiltrarPedidosPorSegragacaoEmpresasFiliaisVinculadasAoUsuario { get; set; }

        public List<int> PedidosSemSessao { get; set; }

        public List<int> CodigosRegiao { get; set; }

        public List<int> CodigosMesorregiao { get; set; }

        public TendenciaEntrega TendenciaEntrega { get; set; }

        public List<int> CodigosCarga { get; set; }

        public bool FiltrarPedidosVinculadoOutrasCarga { get; set; }

        #endregion

        #region Propriedades com Regras

        public double Remetente { set { Remetentes = value > 0 ? new List<double>() { value } : null; } }

        public double Destinatario { set { Destinatarios = value > 0 ? new List<double>() { value } : null; } }

        public double Tomador { set { Tomadores = value > 0 ? new List<double>() { value } : null; } }

        public int Origem { set { CodigosOrigem = value > 0 ? new List<int>() { value } : null; } }

        public int Destino { set { CodigosDestino = value > 0 ? new List<int>() { value } : null; } }

        public int CidadePoloOrigem { set { CodigosCidadePoloOrigem = value > 0 ? new List<int>() { value } : null; } }

        public int CidadePoloDestino { set { CodigosCidadePoloDestino = value > 0 ? new List<int>() { value } : null; } }

        public int PaisDestino { set { CodigosPaisDestino = value > 0 ? new List<int>() { value } : null; } }

        public int PaisOrigem { set { CodigosPaisOrigem = value > 0 ? new List<int>() { value } : null; } }

        public int GrupoPessoa { set { CodigosGrupoPessoa = value > 0 ? new List<int>() { value } : null; } }

        public string EstadoDestino { set { EstadosDestino = !string.IsNullOrWhiteSpace(value) && value != "0" ? new List<string>() { value } : null; } }

        public string EstadoOrigem { set { EstadosOrigem = !string.IsNullOrWhiteSpace(value) && value != "0" ? new List<string>() { value } : null; } }

        #endregion
    }
}
