/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />
/// <reference path="../../Enumeradores/EnumTipoJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoOrdenacaoJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoMontagemCarregamentoVrp.js" />
/// <reference path="../../Enumeradores/EnumTipoOcupacaoMontagemCarregamentoVrp.js" />
/// <reference path="../../Enumeradores/EnumNivelQuebraProdutoRoteirizar.js" />
/// <reference path="../../Enumeradores/EnumLimiteCarregamentosCentroCarregamento.js" />
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="TempoCarregamento.js" />
/// <reference path="CapacidadeCarregamento.js" />
/// <reference path="ControleExpedicao.js" />
/// <reference path="Doca.js" />
/// <reference path="Configuracao.js" />
/// <reference path="Email.js" />
/// <reference path="Geolocalizacao.js" />
/// <reference path="ImportacaoPeriodo.js" />
/// <reference path="ImportacaoPrevisao.js" />
/// <reference path="ImportacaoDisponibilidade.js" />
/// <reference path="ManobraAcao.js" />
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="PrevisaoCarregamento.js" />
/// <reference path="TempoCarregamento.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="CentroCarregamentoTransportador.js" />
/// <reference path="CentroCarregamentoTransportadorTerceiro.js" />
/// <reference path="TransportadoresAutorizadosLiberarFaturamento.js" />
/// <reference path="UsuariosNotificacao.js" />
/// <reference path="Veiculo.js" />
/// <reference path="Lances.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="NaoComparecimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroCarregamento;
var _centroCarregamento;
var _pesquisaCentroCarregamento;
var _crudCentroCarregamento;

var PesquisaCentroCarregamento = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), issue: 586 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557 });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Filial.getFieldDescription(), issue: 70, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga.getFieldDescription(), issue: 53, idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCentroCarregamento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CentroCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, maxlength: 150 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 150 });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 400 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.LimiteCarregamentos = PropertyEntity({ val: ko.observable(EnumLimiteCarregamentosCentroCarregamento.TempoCarregamento), options: EnumLimiteCarregamentosCentroCarregamento.obterOpcoes(), def: EnumLimiteCarregamentosCentroCarregamento.TempoCarregamento, text: Localization.Resources.Logistica.CentroCarregamento.LimiteDeCarregamentos.getFieldDescription() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Filial.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 70, visible: ko.observable(true), required: true });
    this.NumeroDocas = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IndicarTemposVeiculos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Logistica.CentroCarregamento.DesejaExibirOsVeiculosAtrasadosDesteCentroDeCarregamento, issue: 1017 });
    this.VincularMotoristaFilaCarregamentoManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.VincularMotoristaDaFilaDeCarregamentoManualmente, def: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamento) });
    this.TempoBloqueioEscolhaTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.CentroCarregamento.TempoBloqueioEscolhaTransportadorMunitos.getFieldDescription(), issue: 1016 });
    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.TransportadoresAutorizadosLiberarFaturamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), text: Localization.Resources.Logistica.CentroCarregamento.TransportadoresQuePodemAutorizarFaturamento, issue: 1068 });

    this.Emails = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.PeriodosCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.PeriodosDeCarregamento });
    this.PrevisoesCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.PrevisoesDeCarregamento });
    this.DisponibilidadesFrota = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.DisponibilidadeDaFrota });
    this.LimitesCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.LimitesDeCarregamento });
    this.ProdutividadeCarregamentos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() })
    this.PunicoesCarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() })

    this.TempoEmMinutosLiberacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaLiberacaoAposInformarVeiculoMotoristaMinutos.getFieldDescription() });
    this.TempoAguardarConfirmacaoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoAguardarConfirmacaoTransportadorMinutos.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarFilaCarregamento });
    this.TempoEncostaDoca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaVeiculoEnconstarNaDocaMinutos.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarFilaCarregamento });
    this.TempoToleranciaPedidoRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.ToleranciaPedidoRoteirizacaoMinutos.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarFilaCarregamento });
    this.QuantidadeMaximaEntregasRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.MaximoEntregasMontagemCarregamento.getFieldDescription(), visible: ko.observable(true) });//_CONFIGURACAO_TMS.UtilizarFilaCarregamento });
    this.QuantidadeMaximaPedidosSessaoRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.MaximoPedidosPorSessaoRoteirizacao.getFieldDescription(), visible: ko.observable(true) });//_CONFIGURACAO_TMS.UtilizarFilaCarregamento });
    this.UtilizarDispFrotaCentroDescCliente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.UtilizarDisposicaoFrotaCentroDescargaCliente.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    this.ConsiderarTempoDeslocamentoPrimeiraEntrega = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ConsiderarTempoDeslocamentoCD.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarCarregamentosAlemDaDispFrota = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.GerarCarregamentosAlemDaDisponibilidadeDeFrota.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false });
    this.MontagemCarregamentoPedidoProduto = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.MontagemCarregamentoPorPedidoProduto.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    this.MontagemCarregamentoPedidoIntegral = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.MontagemCarregamentoPedidoIntegral.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    //this.MontagemCarregamentoColetaEntrega = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.MontagemCarregamentoPorColetaEntrega.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    this.Latitude = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 100 });
    this.Longitude = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 100 });
    this.DistanciaMinimaEntrarFilaCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.UsuariosNotificacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.PermitirTransportadorInformarValorFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Logistica.CentroCarregamento.PermitirQueTransportadorInformeValorDoFreteNaJanelaDeCarregamento });
    this.ManterComponentesTabelaFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Logistica.CentroCarregamento.SomarOsComponentesDaTabelaDeFreteSeCargaPossuirUmaAoValorDoLanceDoTransportador });
    this.EnviarNotificacoesPorEmail = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarNotificacaoPorEmail, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarNotificacoesCargasRejeitadasPorEmail = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarNotificacoesCargasRejeitadasPorEmail, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailAlertaLeilaoParaTransportadorOfertado = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailAlertaLeilaoParaTransportadorOfertado, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado, val: ko.observable(true), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.UtilizarCapacidadeCarregamentoPorPeso = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoCapacidadeCarregamentoPorPeso = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoPorPeso, val: ko.observable(EnumTipoCapacidadeCarregamentoPorPeso.Todos), def: EnumTipoCapacidadeCarregamentoPorPeso.Todos, options: EnumTipoCapacidadeCarregamentoPorPeso.obterOpcoes(), enable: ko.observable(false) });
    this.PercentualToleranciaPesoCarregamento = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Logistica.CentroCarregamento.PercentualDeToleranciaNoPesoDeCarregamento.getFieldDescription(), visible: ko.observable(true), maxlength: 5, configDecimal: { allowZero: true } });
    this.CargasComoExcedentesNaJanela = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Logistica.CentroCarregamento.AdicionarCargasComoExcedentesNaJanelaDeCarregamento });
    this.ExibirDetalhesCargaJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ExibirDetalhesDaCargaNaJanelaDeCarregamentoDoTransportador, def: false });
    this.OcultarEdicaoDataHora = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.OcultarEdicaoDaDataHora, def: false });
    this.NaoExibirValorFretePortalTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoExibirValorDoFreteNoPortalDoTransportador, def: false });
    this.PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirTransportadorInformeValorFreteCargasDestinadasAEle, def: false });
    this.NaoValidarIntegracaoGR = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoValidarGRParaEntrarNaFilaDeCarregamento, def: false });
    this.PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirSelecaoDoPeriodoDeCarregamentoNaJanelaDeCarregamentoDoTransportador, def: false });
    this.PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirAlterarModeloVeicularDaCargaNaJanelaDeCarregamentoDoTransportador, def: false });
    this.PermitirInformarAreaVeiculoJanelaCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirInformarAsAreasDeVeiculosNaJanelaDeCarregamento, def: false });
    this.GerarGuaritaMesmoSemVeiculoInformado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.GerarGuaritaMesmoQueCargaNaoTenhaVeiculoInformado, def: false });
    this.SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.SeDataInformadaForInferiorDataAtualComoReferenciaDeHorarioInicialDaJanelaDeCarregamento, def: false });
    this.UtilizarNumeroReduzidoDeColunas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.UtilizarNumeroReduzidoDeColunas, def: false });
    this.DiasAdicionaisAlocacaoCargaJanelaCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.DiasAdicionaisParaAlocacaoDecargaNaJanelaDeCarregamento, val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "", maxlength: 2 } });
    this.BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.BloquearInteresseQuantosDiasAntesDeVencerCertificadoDigitalOuApoliceDeSeguroDoTransportador.getFieldDescription(), val: ko.observable(1), def: 1, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "", maxlength: 2 } });
    this.HoraInicioViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: Localization.Resources.Logistica.CentroCarregamento.HoraInicioViagemPrevista.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarDataPrevisaoSaidaVeiculo });
    this.TipoJanelaCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoDaJanelaDeCarregamento.getFieldDescription(), val: ko.observable(EnumTipoJanelaCarregamento.Calendario), options: EnumTipoJanelaCarregamento.obterOpcoes() });
    this.TipoPedidoMontagemCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoDoPedidoDaMontagemCarregamento.getFieldDescription(), val: ko.observable(EnumTipoPedidoMontagemCarregamento.Calendario), options: EnumTipoPedidoMontagemCarregamento.obterOpcoes() });
    
    this.TipoEdicaoPalletProdutoMontagemCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoEdicaoPalletProdutoMontagemCarregamento.getFieldDescription(), val: ko.observable(EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado), options: EnumTipoEdicaoPalletProdutoMontagemCarregamento.obterOpcoes() });

    this.TipoMontagemCarregamentoVRP = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.MontagemCarregamentoOpcao.getFieldDescription(), val: ko.observable(EnumTipoMontagemCarregamentoVrp.Nenhum), options: EnumTipoMontagemCarregamentoVrp.obterOpcoes() });
    this.SimuladorFreteCriterioSelecaoTransportador = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.SimuladorFreteCriterioSelecaoTransportador.getFieldDescription(), val: ko.observable(EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum), options: EnumSimuladorFreteCriterioSelecaoTransportador.obterOpcoes(), visible: ko.observable(false) });

    this.TipoRoteirizacaoColetaEntrega = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.MontagemCarregamentoPorColetaEntrega.getFieldDescription(), val: ko.observable(EnumTipoRoteirizacaoColetaEntrega.Entrega), options: EnumTipoRoteirizacaoColetaEntrega.obterOpcoes(), visible: ko.observable(true) });

    this.TipoResumoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoResumoCarregamento.getFieldDescription(), val: ko.observable(EnumTipoResumoCarregamento.ModeloCargas), options: EnumTipoResumoCarregamento.obterOpcoes() });
    this.TipoOcupacaoMontagemCarregamentoVRP = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.OcupacaoMontagemCarregamento.getFieldDescription(), val: ko.observable(EnumTipoOcupacaoMontagemCarregamentoVrp.Peso), options: EnumTipoOcupacaoMontagemCarregamentoVrp.obterOpcoes(), visible: ko.observable(false) });
    this.NivelQuebraProdutoRoteirizar = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.NivelQuebraProdutoRoteirizar.getFieldDescription(), val: ko.observable(EnumNivelQuebraProdutoRoteirizar.Item), options: EnumNivelQuebraProdutoRoteirizar.obterOpcoes(), visible: ko.observable(false) });
    this.ExibirSomenteJanelaCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ExibirSomenteJanelaDeCarregamento, def: false });
    this.BloquearVeiculoSemEspelhamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.BloquearVeiculoSemEspelhamento, def: false });
    this.BloquearVeiculoSemEspelhamentoTelaCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.BloquearVeiculoSemEspelhamentoTelaCarga, def: false });
    this.EnviarEmailParaTransportadorAoDisponibilizarCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailParaTransportadorAoDisponibilizarCarga, def: false });
    this.TipoOrdenacaoJanelaCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoDaOrdenacaoDaJaneladeCarregamento.getFieldDescription(), val: ko.observable(EnumTipoOrdenacaoJanelaCarregamento.InicioCarregamento), options: EnumTipoOrdenacaoJanelaCarregamento.obterOpcoes(), visible: ko.observable(false) });
    this.TempoMinutosEscolhaAutomaticaCotacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoEmMinutosParaEscolhaAutomaticaDaCotacaoQuemInformarMenorValor.getFieldDescription(), visible: ko.observable(true), maxlength: 5 });
    this.PercentualMaximoDiferencaValorCotacao = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Logistica.CentroCarregamento.PercentualMaximoDeToleranciaDoValorDaCotacao.getFieldDescription(), visible: ko.observable(true), maxlength: 5 });
    this.PercentualMinimoDiferencaValorCotacao = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Logistica.CentroCarregamento.PercentualMinimoDeToleranciaDoValorDaCotacao.getFieldDescription(), visible: ko.observable(true), maxlength: 5 });
    this.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.PontuacaoDescontarDoTransportadorPorEscolhaAutomaticaDaCotacao.getFieldDescription(), visible: ko.observable(true), maxlength: 10 });
    this.LimiteRecorrencia = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.LimiteRecorrenciasDeFreteAcimaDoTeto.getFieldDescription() });
    this.GerarJanelaCarregamentoDestino = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.GerarJanelaDeCarregamentoNoDestino, def: false });
    this.PermitirGeracaoJanelaParaCargaRedespacho = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirGeracaoJanelaParaCargaRedespacho, def: false, visible: _CONFIGURACAO_TMS.BloquearGeracaoJanelaParaCargaRedespacho });
    this.NotificarSomenteAlteracaoCotacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NotificarSomenteAlteracaoCotacao, def: false });
    this.NaoEnviarNotificacaoCargaRejeitadaParaTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador, def: false });
    this.NaoBloquearCapacidadeExcedida = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoBloquearCapacidadeExcedida, def: false });
    this.LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota, def: false, visible: ko.observable(true) });
    this.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido, def: false, visible: ko.observable(true) });
    this.PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento, def: false, visible: ko.observable(true) });
    this.ConsiderarPesoPalletPesoTotalCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ConsiderarPesoPalletPesoTotalCarga, def: false, visible: ko.observable(true) });
    this.PreencherAutomaticamenteDadosCentroTelaMontagemCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga, def: false, visible: ko.observable(true) });
    this.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val.subscribe(function (valor) {
        if (!valor) {
            LimparCamposCentroCarregamentoPadrao();
        }
    });

    this.EmpresaPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Transportador, idBtnSearch: guid(), enable: ko.observable(true), required: false });
    this.TipoOperacaoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao, idBtnSearch: guid(), enable: ko.observable(true), required: false });
    this.VeiculoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Logistica.CentroCarregamento.Veiculo, idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeicularPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Logistica.CentroCarregamento.ModeloVeicular, idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.MotoristaPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.Motorista, idBtnSearch: guid(), visible: ko.observable(true) });

    this.NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga, def: false, visible: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga) });

    this.QuantidadeMinimaEntregasRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Quantidade minima Entregas", visible: ko.observable(true) });

    this.TempoMinutosEscolhaAutomaticaCotacao.val.subscribe(function (valor) {
        _centroCarregamentoConfiguracao.RepassarCargaCasoNaoExistaVeiculoDisponivel.visible(parseFloat(valor) > 0);
    });

    this.CarregamentoTempoMaximoRota = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoMaximoEntreAsEntregasDeUmCarregamentoMinutos.getFieldDescription(), visible: ko.observable(false), maxlength: 5, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.AgruparPedidosMesmoDestinatario = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.AgruparPedidosMesmoDestinatario.getFieldDescription(), visible: ko.observable(false), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false });
    this.GerarCarregamentoDoisDias = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.GerarCarregamentosParaDoisDias.getFieldDescription(), visible: ko.observable(false), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false });

    this.NumeroDocas.val.subscribe(function (novoValor) {
        _capacidadeCarregamento.NumeroDocas.val(novoValor);
    });

    this.TempoEmMinutosLiberacao.val.subscribe(function (novoValor) {
        _transportadoresAutorizadosLiberarFaturamento.TempoEmMinutosLiberacao.val(novoValor);
    });

    this.LimiteCarregamentos.val.subscribe(function (novoValor) {
        _centroCarregamentoConfiguracao.LimiteAlteracoesHorarioTransportador.visible(novoValor === EnumLimiteCarregamentosCentroCarregamento.QuantidadeDocas);
    });

    this.UtilizarCapacidadeCarregamentoPorPeso.val.subscribe(function (novovalor) {
        _capacidadeCarregamento.TipoCapacidadeCarregamento.visible(novovalor);
    });

    this.TipoCapacidadeCarregamentoPorPeso.val.subscribe(controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso);

    this.TipoJanelaCarregamento.val.subscribe(tipoJanelaCarregamentoChange);
    this.TipoMontagemCarregamentoVRP.val.subscribe(tipoMontagemCarregamentoVrpChange);
    this.MontagemCarregamentoPedidoProduto.val.subscribe(montagemCarregamentoPedidoProdutoChange);

    this.CapacidadeCarregamentoSegunda = PropertyEntity({});
    this.CapacidadeCarregamentoTerca = PropertyEntity({});
    this.CapacidadeCarregamentoQuarta = PropertyEntity({});
    this.CapacidadeCarregamentoQuinta = PropertyEntity({});
    this.CapacidadeCarregamentoSexta = PropertyEntity({});
    this.CapacidadeCarregamentoSabado = PropertyEntity({});
    this.CapacidadeCarregamentoDomingo = PropertyEntity({});

    this.CapacidadeCarregamentoCubagemSegunda = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemTerca = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemQuarta = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemQuinta = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemSexta = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemSabado = PropertyEntity({});
    this.CapacidadeCarregamentoCubagemDomingo = PropertyEntity({});

    this.ToleranciaAtrasoSegunda = PropertyEntity({});
    this.ToleranciaAtrasoTerca = PropertyEntity({});
    this.ToleranciaAtrasoQuarta = PropertyEntity({});
    this.ToleranciaAtrasoQuinta = PropertyEntity({});
    this.ToleranciaAtrasoSexta = PropertyEntity({});
    this.ToleranciaAtrasoSabado = PropertyEntity({});
    this.ToleranciaAtrasoDomingo = PropertyEntity({});

    this.TermoAceite = PropertyEntity({});

    this.TipoCapacidadeCarregamento = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int });
}

var CRUDCentroCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadCentroCarregamento() {

    _centroCarregamento = new CentroCarregamento();
    KoBindings(_centroCarregamento, "knockoutDetalhes");
    KoBindings(_centroCarregamento, "knockoutInformacoesPadrao");

    _pesquisaCentroCarregamento = new PesquisaCentroCarregamento();
    KoBindings(_pesquisaCentroCarregamento, "knockoutPesquisaCentroCarregamento", false, _pesquisaCentroCarregamento.Pesquisar.id);

    _crudCentroCarregamento = new CRUDCentroCarregamento();
    KoBindings(_crudCentroCarregamento, "knockoutCRUDCentroCarregamento");

    HeaderAuditoria("CentroCarregamento", _centroCarregamento);

    $("#" + _centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso.id).click(controlarCampoTipoCapacidadeCarregamentoPorPesoHabilitado);

    $("#txtEditor").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });

    BuscarFilial(_centroCarregamento.Filial);
    BuscarFilial(_pesquisaCentroCarregamento.Filial);
    BuscarTiposdeCarga(_pesquisaCentroCarregamento.TipoCarga);

    loadTempoCarregamento();
    LoadTipoCarga();
    LoadCapacidadeCarregamento();
    LoadImportacaoPeriodo();
    LoadImportacaoPrevisao();
    LoadImportacaoDisponibilidade();
    loadCentroCarregamentoTransportador();
    loadCentroCarregamentoTransportadorTerceiro();
    loadoMotorista();
    LoadVeiculo();
    LoadTransportadoresAutorizadosLiberarFaturamento();
    LoadEmail();
    loadControleExpedicao();
    loadGeolocalizacao();
    controlarVisibilidadeAbaGeolocalizacao();
    loadCentroCarregamentoDoca();
    loadCentroCarregamentoConfiguracao();
    loadUsuariosNotificacao();
    loadCentroCarregamentoManobraAcao();
    loadCentroCarregamentoAdvertencia();
    loadLances();
    loadOfertaCarga();
    loadCentroCarregamentoTipoOperacao();
    loadAutomatizacaoNaoComparecimento();
    loadInformacoesTransportador();
    loadProdutividade();
    loadInformacoesJanela();
    loadRetira();
    loadPunicoesCarregamento();

    _centroCarregamento.LimiteCarregamentos.val.subscribe(onCentroCarregamentoLimiteCarregamentosChange);

    BuscarCentroCarregamentos();
    LoadCentroCarregamentoPadrao();
}

function LoadCentroCarregamentoPadrao() {
    KoBindings(_centroCarregamento, "knoutTransportePadrao");

    BuscarTransportadores(_centroCarregamento.EmpresaPadrao);
    BuscarTiposOperacao(_centroCarregamento.TipoOperacaoPadrao);
    BuscarModelosVeicularesCarga(_centroCarregamento.ModeloVeicularPadrao);
    BuscarVeiculos(_centroCarregamento.VeiculoPadrao);
    BuscarMotoristas(_centroCarregamento.MotoristaPadrao);
}

function AdicionarClick() {

    if (ValidarTodosCamposCentroCarregamento()) {
        executarReST("CentroCarregamento/Adicionar", ObterCentroCarregamentoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridCentroCarregamento.CarregarGrid();
                    LimparCamposCentroCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        Global.ResetarAbas();
    }
}

function AtualizarClick() {

    if (ValidarTodosCamposCentroCarregamento()) {
        executarReST("CentroCarregamento/Atualizar", ObterCentroCarregamentoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridCentroCarregamento.CarregarGrid();
                    LimparCamposCentroCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        Global.ResetarAbas();
    }
}

function ExcluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteExcluirCentroDeCarregamento.format(_centroCarregamento.Descricao.val()), function () {
        ExcluirPorCodigo(_centroCarregamento, "CentroCarregamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridCentroCarregamento.CarregarGrid();
                    LimparCamposCentroCarregamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function CancelarClick() {
    LimparCamposCentroCarregamento();
}

function tipoJanelaCarregamentoChange() {
    if (_centroCarregamento.TipoJanelaCarregamento.val() == EnumTipoJanelaCarregamento.Tabela) {
        _centroCarregamento.TipoOrdenacaoJanelaCarregamento.visible(true);
        $("#liInformacoesJanela").hide();
    }
    else {
        _centroCarregamento.TipoOrdenacaoJanelaCarregamento.val(EnumTipoOrdenacaoJanelaCarregamento.InicioCarregamento);
        _centroCarregamento.TipoOrdenacaoJanelaCarregamento.visible(false);
        $("#liInformacoesJanela").show();

    }
}

function tipoMontagemCarregamentoVrpChange() {

    let vrp = (_centroCarregamento.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpCapacity || _centroCarregamento.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);
    let simuladorFrete = (_centroCarregamento.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.SimuladorFrete);

    if (vrp) {
        _centroCarregamento.MontagemCarregamentoPedidoProduto.val(false);
        _centroCarregamento.UtilizarDispFrotaCentroDescCliente.val(false);
    } else {
        _centroCarregamento.SimuladorFreteCriterioSelecaoTransportador.val(EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum);
    }
    _centroCarregamento.SimuladorFreteCriterioSelecaoTransportador.visible(vrp);

    _centroCarregamento.CarregamentoTempoMaximoRota.visible(_centroCarregamento.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);
    _centroCarregamento.ConsiderarTempoDeslocamentoPrimeiraEntrega.visible(_centroCarregamento.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);

    _centroCarregamento.AgruparPedidosMesmoDestinatario.visible(vrp || simuladorFrete);
    _centroCarregamento.GerarCarregamentoDoisDias.visible(vrp);
    _centroCarregamento.TipoOcupacaoMontagemCarregamentoVRP.visible(vrp || simuladorFrete);

    if (simuladorFrete) {
        _centroCarregamento.AgruparPedidosMesmoDestinatario.val(true);
    } else {
        _centroCarregamento.AgruparPedidosMesmoDestinatario.val(false);
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function montagemCarregamentoPedidoProdutoChange() {
    _centroCarregamento.NivelQuebraProdutoRoteirizar.visible(_centroCarregamento.MontagemCarregamentoPedidoProduto.val());
    //_centroCarregamento.MontagemCarregamentoColetaEntrega.visible(!_centroCarregamento.MontagemCarregamentoPedidoProduto.val())
    _centroCarregamento.TipoRoteirizacaoColetaEntrega.visible(!_centroCarregamento.MontagemCarregamentoPedidoProduto.val())
    if (_centroCarregamento.MontagemCarregamentoPedidoProduto.val()) {
        _centroCarregamento.TipoResumoCarregamento.val(EnumTipoResumoCarregamento.SeparacaoProdutos);
    } else {
        _centroCarregamento.TipoResumoCarregamento.val(EnumTipoResumoCarregamento.ModeloCargas);
    }
}

function onCentroCarregamentoLimiteCarregamentosChange() {
    var isCentroCarregamentoControladoPorTempo = IsCentroCarregamentoControladoPorTempo();

    if (isCentroCarregamentoControladoPorTempo) {
        _centroCarregamentoConfiguracao.EscolherHorarioCarregamentoPorLista.visible(false);
        _centroCarregamentoConfiguracao.ExibirVisualizacaoDosTiposDeOperacao.visible(false);
    } else {
        _centroCarregamentoConfiguracao.EscolherHorarioCarregamentoPorLista.visible(true);
        _centroCarregamentoConfiguracao.ExibirVisualizacaoDosTiposDeOperacao.visible(true);
    }

    controlarComponentesVisiveisPorLimiteCarregamentoAlterado();
}


//*******MÉTODOS*******

function IsCentroCarregamentoControladoPorTempo() {
    return _centroCarregamento.LimiteCarregamentos.val() == EnumLimiteCarregamentosCentroCarregamento.TempoCarregamento;
}

function BuscarCentroCarregamentos() {
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (data) { EditarCentroCarregamento(data, false); }, tamanho: "10", icone: "" };
    let duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), evento: "onclick", metodo: function (data) { EditarCentroCarregamento(data, true); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [editar, duplicar], tamanho: 10 };

    _gridCentroCarregamento = new GridView(_pesquisaCentroCarregamento.Pesquisar.idGrid, "CentroCarregamento/Pesquisa", _pesquisaCentroCarregamento, menuOpcoes, null);
    _gridCentroCarregamento.CarregarGrid();
}

function controlarCampoTipoCapacidadeCarregamentoPorPesoHabilitado() {
    if (_centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso.val()) {
        _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.enable(true);

        if (_centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val() == EnumTipoCapacidadeCarregamentoPorPeso.Todos)
            _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val(EnumTipoCapacidadeCarregamentoPorPeso.DiaSemana);
    }
    else {
        _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.enable(false);
        _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val(EnumTipoCapacidadeCarregamentoPorPeso.Todos);
    }
}


function controlarVisibilidadeAbaGeolocalizacao() {
    if (_CONFIGURACAO_TMS.UtilizarFilaCarregamento)
        $("#liGeoLocalizacao").show();
}

function EditarCentroCarregamento(centroCarregamentoGrid, duplicar) {
    LimparCamposCentroCarregamento();
    _centroCarregamento.Codigo.val(centroCarregamentoGrid.Codigo);
    executarReST("CentroCarregamento/BuscarPorCodigo", { Codigo: _centroCarregamento.Codigo.val(), Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_centroCarregamento, retorno);

                _pesquisaCentroCarregamento.ExibirFiltros.visibleFade(false);
                _crudCentroCarregamento.Atualizar.visible(!duplicar);
                _crudCentroCarregamento.Cancelar.visible(true);
                _crudCentroCarregamento.Excluir.visible(!duplicar);
                _crudCentroCarregamento.Adicionar.visible(duplicar);
                controlarCampoTipoCapacidadeCarregamentoPorPesoHabilitado();

                if (retorno.Data.TipoCapacidadeCarregamento != null) {
                    _capacidadeCarregamento.TipoCapacidadeCarregamento.val(retorno.Data.TipoCapacidadeCarregamento);
                    _centroCarregamento.TipoCapacidadeCarregamento.val(retorno.Data.TipoCapacidadeCarregamento);
                }

                carregarGeolocalizacao(_centroCarregamento.DistanciaMinimaEntrarFilaCarregamento.val());
                preencherCentroCarregamentoDoca(retorno.Data.Doca);
                preencherCentroCarregamentoConfiguracao(retorno.Data.Configuracao);
                preencherCentroCarregamentoManobraAcao(retorno.Data.ManobraAcao);
                preencherCentroCarregamentoTransportador(retorno.Data.DadosTransportador);
                preencherCentroCarregamentoTransportadorTerceiro(retorno.Data.DadosTransportadorTerceiro);
                preencherCentroCarregamentoMotorista(retorno.Data.DadosMotorista);
                preencherCentroCarregamentoVeiculo(retorno.Data.DadosVeiculo);
                preencherCentroCarregamentoCamposVisiveisTransportador(retorno.Data.CamposVisiveisTransportador);
                preencherCentroCarregamentoCamposVisiveisJanela(retorno.Data.CamposVisiveisJanela);
                preencherControleExpedicao(retorno.Data.ControleExpedicao);
                preencherLances(retorno.Data.Lances);
                preencherOfertaCarga(retorno.Data.OfertaCarga);
                PreencherEmail(retorno.Data);
                preencherCentroCarregamentoTipoOperacao(retorno.Data.TipoOperacoes);
                preencherCentroCarregamentoNaoComparecimento(retorno.Data.NaoComparecimento);
                preencherRetira(retorno.Data.ObservacaoRetira)

                RecarregarGridTipoCarga();
                RecarregarOpcoesTipoCarga();
                preencherTempoCarregamento(retorno.Data.TemposCarregamento);
                recarregarGridsCapacidadeCarregamento();
                recarregarGridVeiculo();
                RecarregarGridTransportadoresAutorizadosLiberarFaturamento();
                RecarregarGridUsuariosNotificacao();
                recarregarGridProdutividadeCarregamento();
                recarregarGridPunicoesCarregamento();

                habilitarAbaTermoAceite();
                $('#txtEditor').summernote('code', _centroCarregamento.TermoAceite.val());
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function LimparCamposCentroCarregamentoPadrao() {
    _centroCarregamento.EmpresaPadrao.val('');
    _centroCarregamento.VeiculoPadrao.val('');
    _centroCarregamento.ModeloVeicularPadrao.val('');
    _centroCarregamento.TipoOperacaoPadrao.val('');
    _centroCarregamento.MotoristaPadrao.val('');
}

function LimparCamposCentroCarregamento() {
    _crudCentroCarregamento.Atualizar.visible(false);
    _crudCentroCarregamento.Cancelar.visible(false);
    _crudCentroCarregamento.Excluir.visible(false);
    _crudCentroCarregamento.Adicionar.visible(true);

    LimparCampos(_centroCarregamento);
    LimparCamposTipoCarga();
    LimparCamposCapacidadeCarregamento();
    LimparCamposVeiculo();
    LimparCamposTransportadoresAutorizadosLiberarFaturamento();
    LimparCamposEmail();
    limparCamposControleExpedicao();
    limparCamposGeolocalizacao();
    limparCamposCentroCarregamentoDoca();
    limparCamposCentroCarregamentoConfiguracao();
    limparCamposCentroCarregamentoManobraAcao();
    limparCamposCentroCarregamentoTransportador();
    limparCamposCentroCarregamentoTransportadorTerceiro();
    limparCamposCentroCarregamentoMotorista();
    limparCamposCentroCarregamentoVeiculo();
    limparCamposLances();
    limparCamposOfertaCarga();
    limparCamposInformacoesTransportador();
    limparCamposInformacoesJanela();
    limparCamposCentroCarregamentoNaoComparecimento();
    controlarCampoTipoCapacidadeCarregamentoPorPesoHabilitado();
    controlarCampoTipoCapacidadeCarregamento();
    habilitarAbaTermoAceite();
    limparCamposProdutividadeCarregamento();
    limparCamposRetira()
    limparGridPunicoesCarregamento();

    $("#" + _centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso.id).prop("checked", false);

    $('#txtEditor').summernote('code', '');

    recarregarGridProdutividadeCarregamento();
    RecarregarOpcoesTipoCarga();
    limparCamposTempoCarregamento();
    recarregarGridVeiculo();
    RecarregarGridTransportadoresAutorizadosLiberarFaturamento();
    RecarregarGridEmail();
    RecarregarGridUsuariosNotificacao();
    Global.ResetarAbas();
}

function ObterCentroCarregamentoSalvar() {
    _centroCarregamento.TiposCarga.val(JSON.stringify(_tipoCarga.Tipo.basicTable.BuscarRegistros()));
    _centroCarregamento.TransportadoresAutorizadosLiberarFaturamento.val(JSON.stringify(_transportadoresAutorizadosLiberarFaturamento.Transportador.basicTable.BuscarRegistros()));
    _centroCarregamento.Veiculos.val(obterVeiculos());
    _centroCarregamento.UsuariosNotificacao.val(ObterUsuariosNotificacoes());
    _centroCarregamento.EnviarNotificacoesPorEmail.val(_email.EnviarNotificacoesPorEmail.val());
    _centroCarregamento.EnviarNotificacoesCargasRejeitadasPorEmail.val(_email.EnviarNotificacoesCargasRejeitadasPorEmail.val());
    _centroCarregamento.EnviarEmailAlertaLeilaoParaTransportadorOfertado.val(_email.EnviarEmailAlertaLeilaoParaTransportadorOfertado.val());
    _centroCarregamento.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado.val(_email.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado.val());
    _centroCarregamento.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente.val(_email.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente.val());
    _centroCarregamento.TermoAceite.val($('#txtEditor').summernote('code'));
    _centroCarregamento.TipoCapacidadeCarregamento.val(_capacidadeCarregamento.TipoCapacidadeCarregamento.val());

    let centroCarregamento = RetornarObjetoPesquisa(_centroCarregamento);

    preencherTempoCarregamentoSalvar(centroCarregamento);
    preencherCentroCarregamentoDocaSalvar(centroCarregamento);
    preencherCentroCarregamentoConfiguracaoSalvar(centroCarregamento);
    preencherCentroCarregamentoManobraAcaoSalvar(centroCarregamento);
    preencherCentroCarregamentoTransportadorSalvar(centroCarregamento);
    preencherCentroCarregamentoTransportadorTerceiroSalvar(centroCarregamento);
    preencherCentroCarregamentoMotoristaSalvar(centroCarregamento);
    preencherCentroCarregamentoVeiculoSalvar(centroCarregamento);
    preencherControleExpedicaoSalvar(centroCarregamento);
    preencherLancesSalvar(centroCarregamento);
    preencherOfertaCargaSalvar(centroCarregamento);
    preencherCentroCarregamentoTipoOperacaoSalvar(centroCarregamento);
    preencherCentroCarregamentoNaoComparecimentoSalvar(centroCarregamento);
    preencherCamposVisiveisTransportador(centroCarregamento);
    preencherCamposVisiveisJanela(centroCarregamento);
    preencherRetiraSalvar(centroCarregamento)


    return centroCarregamento;
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function ValidarTodosCamposCentroCarregamento() {
    if (!VerificarEmail())
        return false;

    if (!ValidarCamposObrigatorios(_centroCarregamentoConfiguracao))
        return false;

    if (!ValidarCamposObrigatorios(_centroCarregamento))
        return false;

    if (!ValidarCamposObrigatorios(_ofertaCargaKnockout))
        return false;

    if (_ofertaCargaKnockout.AtivarRegraParaOfertarCarga.val()) {
        if (_ofertaCargaKnockout.ListaOfertaCarga.val().length == 0)
        {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.CentroCarregamento.FoiAtivadaRegraDeOfertaDeCargaPorFavorInformePeloMenosUmaRegraNoCadastro);
            return false;
        }
    }

    return true
}