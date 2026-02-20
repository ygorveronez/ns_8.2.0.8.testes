/// <reference path="ResumoCancelamento.js" />
/// <reference path="SignalRCancelamento.js" />
/// <reference path="MDFe.js" />
/// <reference path="CTe.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="Integracao.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/JustificativaCancelamentoCarga.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCargaDocumento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCancelamento;
var _cancelamento;
var _CRUDCancelamento;
var _pesquisaCancelamento;

var PesquisaCancelamento = function () {
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.NumeroCTe.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Operador.getFieldDescription(), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoCarga.Todas), options: EnumSituacaoCancelamentoCarga.ObterOpcoesPesquisa(), def: EnumSituacaoCancelamentoCarga.Todas, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Tipo = PropertyEntity({ val: ko.observable(null), options: EnumTipoCancelamentoCarga.ObterOpcoesPesquisa(), def: null, text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription() });
    this.TipoCancelamentoCargaDocumento = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.TipoCancelamento.getFieldDescription(), val: ko.observable(EnumTipoCancelamentoCargaDocumento.Carga), options: EnumTipoCancelamentoCargaDocumento.obterOpcoesPesquisa(), def: EnumTipoCancelamentoCargaDocumento.Carga, visible: ko.observable(_CONFIGURACAO_TMS.PermitirCancelarDocumentosCargaPeloCancelamentoCarga) });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Cargas.CancelamentoCarga.TipoPessoa.getFieldDescription() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.NumeroOS.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.NumeroBooking.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCarga.NavioViagemDirecao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCarga.TerminalOrigem.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCarga.TerminalDestino.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.CargasLiberadasParaCancelamentoComRejeicaoIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Cargas.CancelamentoCarga.CargasLiberadasParaCancelamentoComRejeicaoIntegracao });


    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _pesquisaCancelamento.Pessoa.visible(true);
            _pesquisaCancelamento.GrupoPessoas.visible(false);
            LimparCampoEntity(_pesquisaCancelamento.GrupoPessoas);
        } else {
            _pesquisaCancelamento.GrupoPessoas.visible(true);
            _pesquisaCancelamento.Pessoa.visible(false);
            LimparCampoEntity(_pesquisaCancelamento.Pessoa);
        }
    });
};

var Cancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.Carga.getRequiredFieldDescription(), issue: 629, idBtnSearch: guid(), eventChange: CargaBlur, enable: ko.observable(true) });
    this.DataCancelamento = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataCancelamento.getRequiredFieldDescription(), issue: 630, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(false) });
    this.UsuarioSolicitou = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Cargas.CancelamentoCarga.UsuarioQueSolicitou.getRequiredFieldDescription(), issue: 631, idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCarga.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.MotivoCancelamento.getRequiredFieldDescription(), issue: 632, maxlength: 255, required: true, enable: ko.observable(true) });
    this.NaoDuplicarCarga = PropertyEntity({ text: (_CONFIGURACAO_TMS.TrocarPreCargaPorCarga ? Localization.Resources.Cargas.CancelamentoCarga.NaoDuplicarCarga : Localization.Resources.Cargas.CancelamentoCarga.NaoRetornarPreCarga), issue: 79899, val: ko.observable(false), def: false, enable: ko.observable(false), visible: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3 input-margin-top-24-lg input-margin-top-24-md") });
    this.MotivoEscape = PropertyEntity();
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCancelamentoCarga.Cancelamento), def: EnumTipoCancelamentoCarga.Cancelamento, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoCarga.AgConfirmacao), def: EnumSituacaoCancelamentoCarga.AgConfirmacao, getType: typesKnockout.int });
    this.SituacaoMDFes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoAverbacaoMDFes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoCTes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoInutilizacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoAverbacaoCTes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoIntegracoes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoValePedagio = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoCIOT = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.GerouIntegracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.JustificativaCancelamentoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && !_CONFIGURACAO_TMS.ObrigarJustificativaCancelamentoCarga ? "" : "*") + Localization.Resources.Cargas.CancelamentoCarga.JustificativaCancelamentoCarga.getFieldDescription(), issue: 0, idBtnSearch: guid(), eventChange: JustificativaCancelamentoCargaBlur, enable: ko.observable(true), visible: ko.observable(false) });
    this.OperadorResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(_CONFIGURACAO_TMS.ObrigatorioOperadorResponsavelCancelamentoCarga), text: (_CONFIGURACAO_TMS.ObrigatorioOperadorResponsavelCancelamentoCarga ? "*" : "") + Localization.Resources.Cargas.CancelamentoCarga.OperadorResponsavel.getFieldDescription(), issue: 0, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoCancelamentoCargaDocumento = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCarga.TipoCancelamento.getFieldDescription(), val: ko.observable(EnumTipoCancelamentoCargaDocumento.Carga), options: EnumTipoCancelamentoCargaDocumento.obterOpcoes(), def: EnumTipoCancelamentoCargaDocumento.Carga, enable: ko.observable(true), visible: ko.observable(false) });
    this.CTeParaCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Cargas.CancelamentoCarga.DocumentoCancelar.getFieldDescription(), issue: 629, idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.Motivo.val.subscribe(function (novoValor) {
        const regex = new RegExp(_CONFIGURACAO_TMS.ReplaceMotivoRegexPattern, "gi");

        // Remove os caracteres não permitidos
        _cancelamento.Motivo.val(novoValor.replace(regex, ""));
    });

    this.TipoCancelamentoCargaDocumento.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoCancelamentoCargaDocumento.Carga) {
            _cancelamento.CTeParaCancelamento.visible(false);
            _cancelamento.CTeParaCancelamento.required(false);
        } else if (novoValor == EnumTipoCancelamentoCargaDocumento.Documentos) {
            _cancelamento.CTeParaCancelamento.visible(true);
            _cancelamento.CTeParaCancelamento.required(true);
        }
        else if (novoValor == EnumTipoCancelamentoCargaDocumento.TodosDocumentos) {
            _cancelamento.CTeParaCancelamento.visible(false);
            _cancelamento.CTeParaCancelamento.required(false);
            _cancelamento.CTeParaCancelamento.codEntity(0);
        }
    });

    this.Anexos = PropertyEntity({ eventClick: AbrirTelaAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true), icon: "fa fa-file-zip-o" });
};

var CRUDCancelamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.CancelamentoCarga.GerarCancelamento), visible: ko.observable(true) });
    this.GerarNovoCancelamento = PropertyEntity({ eventClick: GerarNovoCancelamentoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.GerarNovoCancelamento, visible: ko.observable(false) });
    this.AprovarCancelamento = PropertyEntity({ eventClick: AprovarCancelamentoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.Aprovar, visible: ko.observable(false) });
    this.RejeitarCancelamento = PropertyEntity({ eventClick: ReprovarCancelamentoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.Reprovar, visible: ko.observable(false) });
    this.ReenviarCancelamento = PropertyEntity({ eventClick: ReenviarCancelamentoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.ReenviarCancelamento, visible: ko.observable(false) });
    this.AdicionarComoCancelamento = PropertyEntity({ eventClick: AdicionarComoCancelamentoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.AdicionarCancelamento, visible: ko.observable(false) });
    this.ReenviarCancelamentoComoAnulacao = PropertyEntity({ eventClick: ReenviarCancelamentoComoAnulacaoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.AnularCarga, visible: ko.observable(false) });
    this.LiberarCancelamentoComAverbacaoMDFeRejeitada = PropertyEntity({ eventClick: LiberarCancelamentoComAverbacaoMDFeRejeitadaClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.LiberarCancelamentoMDFe, visible: ko.observable(false) });
    this.LiberarCancelamentoComAverbacaoCTeRejeitada = PropertyEntity({ eventClick: LiberarCancelamentoComAverbacaoCTeRejeitadaClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.LiberarCancelamentoAverbacaoCTe, visible: ko.observable(false) });
    this.LiberarCancelamentoComValePedagioRejeitado = PropertyEntity({ eventClick: LiberarCancelamentoComValePedagioRejeitadoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.LiberarSemCancelarValePedagio, visible: ko.observable(false) });
    this.LiberarCancelamentoComCTeNaoInutilizado = PropertyEntity({ eventClick: LiberarCancelamentoComCTeNaoInutilizadoClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.LiberarSemInutilizarCTes, visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCancelamento() {

    _cancelamento = new Cancelamento();
    KoBindings(_cancelamento, "knockoutCadastroCancelamento");

    _CRUDCancelamento = new CRUDCancelamento();
    KoBindings(_CRUDCancelamento, "knockoutCRUDCancelamento");

    _pesquisaCancelamento = new PesquisaCancelamento();
    KoBindings(_pesquisaCancelamento, "knockoutPesquisaCancelamento", false, _pesquisaCancelamento.Pesquisar.id);

    HeaderAuditoria("CargaCancelamento", _cancelamento);

    new BuscarFuncionario(_pesquisaCancelamento.Operador);
    new BuscarClientes(_pesquisaCancelamento.Pessoa);
    new BuscarGruposPessoas(_pesquisaCancelamento.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarPedidoViagemNavio(_pesquisaCancelamento.PedidoViagemDirecao);
    new BuscarTipoTerminalImportacao(_pesquisaCancelamento.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaCancelamento.TerminalDestino);
    new BuscarLocalidades(_pesquisaCancelamento.Origem);
    new BuscarLocalidades(_pesquisaCancelamento.Destino);

    new BuscarJustificativas(_cancelamento.Justificativa);
    new BuscarOperador(_cancelamento.OperadorResponsavel);
    new BuscarJustificativaCancelamentoCarga(_cancelamento.JustificativaCancelamentoCarga, RetornoJustificativaCancelamento);
    new BuscarCargas(_cancelamento.Carga, RetornoConsultaCarga, null, null, null, [EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.EmCancelamento, EnumSituacoesCarga.Anulada], null, null, null, null, true, null, null, null, null, null, true);
    new BuscarCTesComCarga(_cancelamento.CTeParaCancelamento, null, _cancelamento.Carga);

    LoadEtapaCancelamento();
    LoadIntegracaoDadosCancelamento();
    LoadIntegracaoDadosCancelamentoCTe();
    LoadCIOT();
    LoadMDFe();
    LoadCTe();
    LoadResumoCancelamento();
    LoadSignalRCancelamento();
    LoadIntegracoes();
    LoadAnexo();
    //loadIntegracao();

    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        $("#divPesquisaMultimodal").hide();
    }
    else {
        _cancelamento.Justificativa.visible(true);
        _cancelamento.Justificativa.required = true;
    }

    if (!_CONFIGURACAO_TMS.ExibirJustificativaCancelamentoCarga) {
        _cancelamento.JustificativaCancelamentoCarga.required = false;
        _cancelamento.JustificativaCancelamentoCarga.visible(false);
    } else {
        _cancelamento.JustificativaCancelamentoCarga.required = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? _CONFIGURACAO_TMS.ObrigarJustificativaCancelamentoCarga : true;
        _cancelamento.JustificativaCancelamentoCarga.visible(true);
    }

    BuscarCancelamentos();

    ObterDadosGerais();

    var url = window.location.href;
    var arguments = url.split('#')[1].split('=');
    if (arguments != null && arguments != undefined && arguments.length > 0) {

        CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA = arguments[1];

        if (CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA > 0) {
            ValidarCargaTelaCarga();
            window.history.pushState({}, document.title, url.split("?")[0]);
        }
    }
}

function RetornoJustificativaCancelamento(data) {
    _cancelamento.JustificativaCancelamentoCarga.codEntity(data.Codigo);
    _cancelamento.JustificativaCancelamentoCarga.val(data.Descricao);

    _cancelamento.Motivo.val(data.MotivoCancelamento);
}

function ObterDadosGerais() {
    executarReST("CancelamentoCarga/ObterDadosGerais", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _cancelamento.UsuarioSolicitou.val(r.Data.Usuario);
                _cancelamento.UsuarioSolicitou.def = r.Data.Usuario;
                _cancelamento.UsuarioSolicitou.codEntity(r.Data.CodigoUsuario);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });

    ControleExibicaoNaoDuplicarCarga();
}

function ValidarFaturamentoCarga(e, sender) {
    return new Promise((resolve, reject) => {
        executarReST("CancelamentoCarga/ObterFaturasFechadas", { Carga: _cancelamento.Carga.codEntity, }, function (r) {
            if (r.Success) {
                if (r.Data && r.Data.ContemFaturasFechadas != null && r.Data.ContemFaturasFechadas == true) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoCTeCarga, function () {
                        if (r.Data.FaturaComOutrasCarga != null && r.Data.FaturaComOutrasCarga == true) {
                            exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoFaturaComOutrosCTesVinculados, function () {
                                resolve(true);
                            }, function () {
                                resolve(false);
                            });
                        } else {
                            resolve(true);
                        }
                    }, function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } else {
                resolve(true);
            }
        });
    });
}
function AdicionarClick(e, sender) {

    ValidarFaturamentoCarga(e, sender).then((validarFaturamentoCarga) => {
        if (!validarFaturamentoCarga) {
            return;
        }
        executarReST("CancelamentoCarga/ObterCargasVinculadas", { Carga: _cancelamento.Carga.codEntity }, function (r) {
            if (r.Success) {
                if (r.Data && r.Data.CargasVinculadas != null) {
                    exibirConfirmacao(
                        Localization.Resources.Gerais.Geral.Atencao,
                        Localization.Resources.Cargas.CancelamentoCarga.CargaVinculada +
                        r.Data.CargasVinculadas +
                        Localization.Resources.Cargas.CancelamentoCarga.DesejaRealmenteCancelar,
                        function () {

                            _cancelamento.MotivoEscape.val(encodeURIComponent(_cancelamento.Motivo.val()));
                            Salvar(_cancelamento, "CancelamentoCarga/Adicionar", function (r) {
                                if (r.Success) {
                                    if (r.Data) {
                                        EnviarArquivosAnexados(r.Data).then(function () {
                                            exibirMensagem(
                                                tipoMensagem.ok,
                                                Localization.Resources.Gerais.Geral.Sucesso,
                                                (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento
                                                    ? Localization.Resources.Cargas.CancelamentoCarga.CancelamentoSolicitado
                                                    : Localization.Resources.Cargas.CancelamentoCarga.AnulacaoSolicitada) +
                                                Localization.Resources.Cargas.CancelamentoCarga.ComSucesso
                                            );
                                            _cancelamento.Codigo.val(r.Data);
                                            BuscarCancelamentoPorCodigo();
                                        });
                                    } else {
                                        exibirMensagem(
                                            tipoMensagem.atencao,
                                            Localization.Resources.Gerais.Geral.Atencao,
                                            r.Msg,
                                            10000
                                        );
                                    }
                                } else {
                                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                                }
                            }, sender);
                        }
                    );
                } else {
                    _cancelamento.MotivoEscape.val(encodeURIComponent(_cancelamento.Motivo.val()));
                    Salvar(_cancelamento, "CancelamentoCarga/Adicionar", function (r) {
                        if (r.Success) {
                            if (r.Data) {
                                EnviarArquivosAnexados(r.Data).then(function () {
                                    exibirMensagem(
                                        tipoMensagem.ok,
                                        Localization.Resources.Gerais.Geral.Sucesso,
                                        (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento
                                            ? Localization.Resources.Cargas.CancelamentoCarga.CancelamentoSolicitado
                                            : Localization.Resources.Cargas.CancelamentoCarga.AnulacaoSolicitada) +
                                        Localization.Resources.Cargas.CancelamentoCarga.ComSucesso
                                    );
                                    _cancelamento.Codigo.val(r.Data);
                                    BuscarCancelamentoPorCodigo();
                                });
                            } else {
                                exibirMensagem(
                                    tipoMensagem.atencao,
                                    Localization.Resources.Gerais.Geral.Atencao,
                                    r.Msg,
                                    10000
                                );
                            }
                        } else {
                            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                        }
                    }, sender);
                }
            }
        });
    });
}


function AprovarCancelamentoClick(e, sender) {
    executarReST("CancelamentoCarga/Aprovar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento ? Localization.Resources.Cargas.CancelamentoCarga.CancelamentoAprovado : Localization.Resources.Cargas.CancelamentoCarga.AnulacaoAprovada) + Localization.Resources.Cargas.CancelamentoCarga.ComSucesso);
                BuscarCancelamentoPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReprovarCancelamentoClick(e, sender) {
    executarReST("CancelamentoCarga/Reprovar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridCancelamento.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento ? Localization.Resources.Cargas.CancelamentoCarga.CancelamentoReprovado : Localization.Resources.Cargas.CancelamentoCarga.AnulacaoReprovada) + Localization.Resources.Cargas.CancelamentoCarga.ComSucesso);
                LimparCamposCancelamento();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarCancelamentoClick(e, sender) {
    executarReST("CancelamentoCarga/Reenviar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento ? Localization.Resources.Cargas.CancelamentoCarga.CancelamentoReenviado : Localization.Resources.Cargas.CancelamentoCarga.AnulacaoReenviada) + Localization.Resources.Cargas.CancelamentoCarga.ComSucesso);
                BuscarCancelamentoPorCodigo(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function AdicionarComoCancelamentoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.AdicionarCargaParaCancelamento, function () {
        _cancelamento.MotivoEscape.val(encodeURIComponent(_cancelamento.Motivo.val()));
        Salvar(_cancelamento, "CancelamentoCarga/AdicionarComoCancelamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    EnviarArquivosAnexados(r.Data).then(function () {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoSolicitado);
                        _cancelamento.Codigo.val(r.Data);
                        BuscarCancelamentoPorCodigo();
                    });
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg, 10000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        }, sender);
    });
}

function ReenviarCancelamentoComoAnulacaoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.DesejaAnularCarga, function () {
        executarReST("CancelamentoCarga/ReenviarCancelamentoComoAnulacao", { Codigo: _cancelamento.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CancelamentoCarga.AnulacaoSolicitada);
                    BuscarCancelamentoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function CargaBlur() {
    if (_cancelamento.Carga.val() == "")
        _cancelamento.Carga.codEntity(0);
}

function JustificativaCancelamentoCargaBlur() {
    if (_cancelamento.JustificativaCancelamentoCarga.val() == "")
        _cancelamento.JustificativaCancelamentoCarga.codEntity(0);
}

function RetornoConsultaCarga(data) {
    $("#msgInfoCancelamento").addClass("d-none");
    _cancelamento.Carga.codEntity(data.Codigo);
    _cancelamento.Carga.val(data.CodigoCargaEmbarcador);
    ObterDetalhesCarga(data.Codigo);
}

function ObterDetalhesCarga(codigoCarga) {
    executarReST("CancelamentoCarga/ValidarCarga", { Carga: codigoCarga }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;

                _cancelamento.Tipo.val(r.Data.TipoCancelamento);

                _CRUDCancelamento.Adicionar.visible(true);
                if (r.Data.TipoCancelamento == EnumTipoCancelamentoCarga.Anulacao) {

                    _CRUDCancelamento.Adicionar.text(Localization.Resources.Cargas.CancelamentoCarga.GerarAnulacao);

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_Anular, _PermissoesPersonalizadasCancelamentoCarga))
                        _CRUDCancelamento.Adicionar.visible(false);

                    var msg = "";

                    if (!r.Data.MDFePermiteCancelamento)
                        msg += "Os MDF-es";
                    else if (!r.Data.CTePermiteCancelamento)
                        msg += "Os CT-es";
                    else if (!r.Data.AverbacaoPermiteCancelamento)
                        msg += Localization.Resources.Cargas.CancelamentoCarga.Documentos;
                    else if (!r.Data.CiotPermiteCancelamento)
                        msg += Localization.Resources.Cargas.CancelamentoCarga.Documentos;

                    msg += " " + Localization.Resources.Cargas.CancelamentoCarga.NaoPodemSerCancelados;

                    $("#msgInfoCancelamento").text(msg);
                    $("#msgInfoCancelamento").removeClass("d-none");

                    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento, _PermissoesPersonalizadasCancelamentoCarga))
                        _CRUDCancelamento.AdicionarComoCancelamento.visible(true);

                } else {
                    if (r.Data.DocumentoEmitidoNoEmbarcador === true) {
                        _CRUDCancelamento.Adicionar.text(Localization.Resources.Cargas.CancelamentoCarga.DisponibilizarVincularOutraCarga);

                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_PermitirDisponibilizarCTesParaVincularEmOutraCarga, _PermissoesPersonalizadasCancelamentoCarga))
                            _CRUDCancelamento.Adicionar.visible(true);
                        else
                            _CRUDCancelamento.Adicionar.visible(false);

                        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento, _PermissoesPersonalizadasCancelamentoCarga))
                            _CRUDCancelamento.AdicionarComoCancelamento.visible(true);
                    } else {
                        _CRUDCancelamento.Adicionar.text(Localization.Resources.Cargas.CancelamentoCarga.GerarCancelamento);
                    }
                }

                _cancelamento.TipoCancelamentoCargaDocumento.val(EnumTipoCancelamentoCargaDocumento.Carga);
                _cancelamento.TipoCancelamentoCargaDocumento.visible(_CONFIGURACAO_TMS.PermitirCancelarDocumentosCargaPeloCancelamentoCarga && data.SituacaoPermiteCancelarApenasDocumentos);

                _cancelamento.TipoCancelamentoCargaDocumento.val(r.Data.TipoCancelamentoCarga);
                _cancelamento.TipoCancelamentoCargaDocumento.enable(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function GerarNovoCancelamentoClick(e) {
    LimparCamposCancelamento();
}

//*******MÉTODOS*******

function BuscarCancelamentos() {
    var configExportacao = {
        url: "CancelamentoCarga/ExportarPesquisa",
        titulo: Localization.Resources.Cargas.CancelamentoCarga.CancelamentoDeCarga
    };

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarCancelamento, tamanho: "7", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCancelamento = new GridViewExportacao(_pesquisaCancelamento.Pesquisar.idGrid, "CancelamentoCarga/Pesquisa", _pesquisaCancelamento, menuOpcoes, configExportacao, { column: 1, dir: orderDir.desc });
    _gridCancelamento.CarregarGrid();
}

function EditarCancelamento(cancelamentoGrid) {
    LimparCamposCancelamento();

    _cancelamento.Codigo.val(cancelamentoGrid.Codigo);

    BuscarCancelamentoPorCodigo();
}

function BuscarCancelamentoPorCodigo(exibirLoading) {

    if (exibirLoading == null)
        exibirLoading = true;

    if (!exibirLoading)
        _ControlarManualmenteProgresse = true;

    BuscarPorCodigo(_cancelamento, "CancelamentoCarga/BuscarPorCodigo", function (arg) {
        _ControlarManualmenteProgresse = false;

        _cancelamento.Carga.codEntity(arg.Data.Carga.Codigo);
        _cancelamento.Carga.val(arg.Data.Carga.CodigoCargaEmbarcador);
        _cancelamento.Motivo.val(arg.Data.MotivoEscape);
        _cancelamento.CTeParaCancelamento.enable(false);
        _cancelamento.CTeParaCancelamento.val(arg.Data.CTeParaCancelamento);
        _cancelamento.UsuarioSolicitou.val(arg.Data.UsuarioSolicitou);

        _anexo.Anexos.val(arg.Data.Anexos);
        _anexo.Anexos.visible(false);
        _anexo.Finalizar.visible(false);

        PreecherResumoCancelamento(arg.Data);
        PreecherCamposEdicaoCancelamento();
    }, null, exibirLoading);
}

function PreecherCamposEdicaoCancelamento() {
    _pesquisaCancelamento.ExibirFiltros.visibleFade(false);

    _CRUDCancelamento.GerarNovoCancelamento.visible(true);
    _CRUDCancelamento.Adicionar.visible(false);
    _CRUDCancelamento.AdicionarComoCancelamento.visible(false);
    _CRUDCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada.visible(false);
    _CRUDCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada.visible(false);
    _CRUDCancelamento.LiberarCancelamentoComCTeNaoInutilizado.visible(false);
    _CRUDCancelamento.LiberarCancelamentoComValePedagioRejeitado.visible(false);

    if (_cancelamento.Situacao.val() == EnumSituacaoCancelamentoCarga.AgConfirmacao) {
        _CRUDCancelamento.AprovarCancelamento.visible(true);
        _CRUDCancelamento.RejeitarCancelamento.visible(true);

        ObterDetalhesCarga(_cancelamento.Carga.codEntity());
    }
    else {
        _CRUDCancelamento.AprovarCancelamento.visible(false);
        _CRUDCancelamento.RejeitarCancelamento.visible(false);
    }

    if (_cancelamento.Situacao.val() == EnumSituacaoCancelamentoCarga.RejeicaoCancelamento) {
        _CRUDCancelamento.ReenviarCancelamento.visible(true);

        if (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_ReenviarCancelamentoComoAnulacao, _PermissoesPersonalizadasCancelamentoCarga))
            _CRUDCancelamento.ReenviarCancelamentoComoAnulacao.visible(true);
        else
            _CRUDCancelamento.ReenviarCancelamentoComoAnulacao.visible(false);

        if (_cancelamento.Tipo.val() == EnumTipoCancelamentoCarga.Cancelamento) {
            if (_cancelamento.SituacaoMDFes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && _cancelamento.SituacaoAverbacaoMDFes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCancelamentoCarga))
                _CRUDCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada.visible(true);
        }

        if (_cancelamento.SituacaoCTes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && _cancelamento.SituacaoAverbacaoCTes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCancelamentoCarga))
            _CRUDCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada.visible(true);

        if (_cancelamento.SituacaoCTes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && _cancelamento.SituacaoValePedagio.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCancelamentoCarga))
            _CRUDCancelamento.LiberarCancelamentoComValePedagioRejeitado.visible(true);

        if (_cancelamento.SituacaoCTes.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && _cancelamento.SituacaoInutilizacao.val() == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado, _PermissoesPersonalizadasCancelamentoCarga))
            _CRUDCancelamento.LiberarCancelamentoComCTeNaoInutilizado.visible(true);
    }
    else if (_cancelamento.Situacao.val() == EnumSituacaoCancelamentoCarga.SolicitacaoReprovada) {
        _CRUDCancelamento.ReenviarCancelamento.visible(true);
        _CRUDCancelamento.ReenviarCancelamentoComoAnulacao.visible(false);
    }
    else {
        _CRUDCancelamento.ReenviarCancelamento.visible(false);
        _CRUDCancelamento.ReenviarCancelamentoComoAnulacao.visible(false);
    }

    _cancelamento.Justificativa.enable(false);
    _cancelamento.Motivo.enable(false);
    _cancelamento.Carga.enable(false);
    _cancelamento.NaoDuplicarCarga.enable(false);
    _cancelamento.JustificativaCancelamentoCarga.enable(false);
    _cancelamento.OperadorResponsavel.enable(false);
    _cancelamento.TipoCancelamentoCargaDocumento.enable(false);
    _cancelamento.TipoCancelamentoCargaDocumento.visible(_CONFIGURACAO_TMS.PermitirCancelarDocumentosCargaPeloCancelamentoCarga);

    ConsultarCTesCarga();
    ConsultarMDFesCarga();
    //ConsultarIntegracao();
    SetarEtapaCancelamento();
};

function LimparCamposCancelamento() {
    _cancelamento.Motivo.val("");
    _CRUDCancelamento.GerarNovoCancelamento.visible(false);
    _CRUDCancelamento.Adicionar.visible(true);
    _CRUDCancelamento.AprovarCancelamento.visible(false);
    _CRUDCancelamento.RejeitarCancelamento.visible(false);
    _CRUDCancelamento.ReenviarCancelamento.visible(false);
    _CRUDCancelamento.ReenviarCancelamentoComoAnulacao.visible(false);
    _CRUDCancelamento.AdicionarComoCancelamento.visible(false);

    _CRUDCancelamento.Adicionar.text(Localization.Resources.Cargas.CancelamentoCarga.GerarCancelamento);

    _cancelamento.Justificativa.enable(true);
    _cancelamento.Motivo.enable(true);
    _cancelamento.Carga.enable(true);
    _cancelamento.JustificativaCancelamentoCarga.enable(false);
    _cancelamento.OperadorResponsavel.enable(true);
    _cancelamento.TipoCancelamentoCargaDocumento.enable(true);
    _cancelamento.TipoCancelamentoCargaDocumento.visible(false);

    $("#msgInfoCancelamento").addClass("d-none");

    LimparCampos(_cancelamento);
    LimparCamposCTe();
    LimparCamposMDFe();
    LimparResumoCancelamento();
    LimparCamposAnexo();
    SetarEtapaInicioCancelamento();
    ControleExibicaoNaoDuplicarCarga();
}

function ControleExibicaoNaoDuplicarCarga() {
    if (!_CONFIGURACAO_TMS.TrocarPreCargaPorCarga) {
        _cancelamento.NaoDuplicarCarga.visible(true);
        _cancelamento.NaoDuplicarCarga.enable(true);
        _cancelamento.Justificativa.enable(true);
        _cancelamento.JustificativaCancelamentoCarga.enable(true);
        _cancelamento.NaoDuplicarCarga.val(false);
    }
    else if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.SempreDuplicarCargaCancelada) && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_NaoDuplicarCarga, _PermissoesPersonalizadasCancelamentoCarga)) {
        _cancelamento.NaoDuplicarCarga.visible(true);
        _cancelamento.NaoDuplicarCarga.enable(true);
        _cancelamento.Justificativa.enable(true);
        _cancelamento.JustificativaCancelamentoCarga.enable(true);
        _cancelamento.NaoDuplicarCarga.val(_CONFIGURACAO_TMS.DefaultTrueDuplicarCarga);
    }
    else {
        _cancelamento.NaoDuplicarCarga.visible(false);
        _cancelamento.NaoDuplicarCarga.enable(false);
        _cancelamento.NaoDuplicarCarga.val(false);
        _cancelamento.JustificativaCancelamentoCarga.enable(true);
    }
}

function ValidarCargaTelaCarga() {
    executarReST("CancelamentoCarga/ValidarCargaTelaCarga", { CodigoCarga: CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _cancelamento.Carga.val(r.Data.Carga.Descricao);
                _cancelamento.Carga.codEntity(r.Data.Carga.Codigo);
                _cancelamento.Carga.enable(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });

    ControleExibicaoNaoDuplicarCarga();
}