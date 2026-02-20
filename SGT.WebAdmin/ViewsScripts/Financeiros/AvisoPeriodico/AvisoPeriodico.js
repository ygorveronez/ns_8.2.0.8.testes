/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />

//#region Variaveis Globais
var _pesquisaAvisoPeriodico;
var _gridAvisoPeriodico;
var _gridFiliais;
var pendenciasModuloControle;


//#endregion


//#region Constructores

function PesquisaAvisoPeriodico() {
    this.NumeroAviso = PropertyEntity({ text: "Número do aviso: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAvisoPeriodico.Todas), options: EnumSituacaoAvisoPeriodico.obterOpcoesPesquisa(), def: EnumSituacaoAvisoPeriodico.Todas });
    this.DataGeracaoInicial = PropertyEntity({ text: "Data geração inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataGeracaoFinal = PropertyEntity({ text: "Data geração final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataPeriodoInicial = PropertyEntity({ text: "Data período inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataPeriodoFinal = PropertyEntity({ text: "Data período final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAvisoPeriodico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Confirmar = PropertyEntity({ text: 'Confirmar', eventClick: confirmarAvisoClick, type: types.event, idGrid: guid(), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ text: 'Rejeitar', eventClick: rejeitarAvisoClick, type: types.event, visible: ko.observable(false), enable: ko.observable(true) });
}

function AvisoPeriodico() {
    this.NumeroAviso = PropertyEntity({ text: "Número Termo: ", val: ko.observable(0) })
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(false) });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PagamentosEDescontosViaCreditoEmConta = PropertyEntity({ text: "Pagamentos e descontos via crédito em conta: ", val: ko.observable("R$ 0,00") });
    this.PagamentosEDescontosViaConfirming = PropertyEntity({ text: "Pagamentos e descontos via confirming: ", val: ko.observable("R$ 0,00") });
    this.CreditoEmConta = PropertyEntity({ text: "Crédito em Conta: ", val: ko.observable("R$ 0,00") });

    this.TotalAdiantamento = PropertyEntity({ text: "Total Adiantamento: ", val: ko.observable("R$ 0,00") });
    this.NotasCompensadasAdiantamentos = PropertyEntity({ text: "Notas Compensadas Contra Adiantamentos: ", val: ko.observable("R$ 0,00") });
    this.SaldoAdiantamentoEmAberto = PropertyEntity({ text: "Saldo do adiantamento em aberto: ", val: ko.observable("R$ 0,00") });

    this.TotalGeralPagamentos = PropertyEntity({ text: "Total Geral dos Pagamentos: ", val: ko.observable("R$ 0,00") });

    this.AvariasEmAberto = PropertyEntity({ text: "Avarias em aberto: ", val: ko.observable("R$ 0,00") });
    this.DebitosBaixaResultado = PropertyEntity({ text: "Débitos baixa resultado: ", val: ko.observable("R$ 0,00") });
    this.TotalCompensacoes = PropertyEntity({ text: "Total de compensações: ", val: ko.observable("R$ 0,00") });

    this.TotalVencidoTransportador = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalAVencerTransportador = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalVencidoDesbloqueado = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalAVencerDesbloqueado = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalVencidoUnilever = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalAVencerUnilever = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalVencidoBloqueioPOD = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalAVencerBloqueioPOD = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendentesVencidaBloqueioIrregularidade = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendentesAVencerBloqueioIrregularidade = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendentesBloqueioIrregularidade = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendenciasBloqueioPOD = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendenciasUnilever = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendenciasDesbloqueado = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendenciasTransportador = PropertyEntity({ val: ko.observable("R$ 0,00") });
    this.TotalPendentes = PropertyEntity({ text: "Total de pendentes: ", val: ko.observable("R$ 0,00") });

    this.TotalUnilever = PropertyEntity({ text: "Unilever", val: ko.observable("R$ 0,00") });
    this.ProjecaoRecebimento = PropertyEntity({ text: "Projeção de Recebimento: ", val: ko.observable("R$ 0,00") });
 

    this.ExportarResumo = PropertyEntity({ text: 'Exportar Resumo', eventClick: exportarResumoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ text: 'Confirmar', eventClick: confirmarAvisoClick, type: types.event, idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) });
    this.Rejeitar = PropertyEntity({ text: 'Rejeitar', eventClick: rejeitarAvisoClick, type: types.event, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe), enable: ko.observable(true) });
}


function RejeitarAviso() {
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", val: ko.observable(""), required: ko.observable(true), maxlength: 1000 });
    this.Enviar = PropertyEntity({ text: 'Enviar', eventClick: enviarRejeitarAvisoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });

    this.Justificativa.val.subscribe(function (novoValor) {
        $(`#${_modalRejeitarAviso.Justificativa.id}`).removeClass("is-invalid")
    });
}

//#endregion


//#region Funções de Carregamento
function loadAvisoPeriodico() {
    _pesquisaAvisoPeriodico = new PesquisaAvisoPeriodico();
    KoBindings(_pesquisaAvisoPeriodico, "knockoutPesquisaAvisoPeriodico");

    _avisoPeriodico = new AvisoPeriodico();
    KoBindings(_avisoPeriodico, "divModalDetalhesAvisoPeriodico");

    _modalRejeitarAviso = new RejeitarAviso();
    KoBindings(_modalRejeitarAviso, "knoutModalRejeitarAviso");

    loadGridAvisoPeriodico();
    loadGridFiliais();

    new BuscarTransportadores(_pesquisaAvisoPeriodico.Transportador);
}
//#endregion


//#region Funções Auxiliares

function loadGridAvisoPeriodico() {
        var detalhesRegistro = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalhesRegistroClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhesRegistro]
    }

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        callbackNaoSelecionado: verificarCallBack,
        callbackSelecionado: verificarCallBack,
        somenteLeitura: false
    };

    _gridAvisoPeriodico = new GridView(_pesquisaAvisoPeriodico.Pesquisar.idGrid, "AvisoPeriodico/Pesquisa", _pesquisaAvisoPeriodico, menuOpcoes, null, null, null, null, null, multiplaescolha);
    _gridAvisoPeriodico.CarregarGrid();

};

function gridBasicFiliais(idGrid) {

    var header = [
        { data: "CodigoIntegracao", title: "Código Integração", width: "15%", className: "text-align-center" },
        { data: "CNPJ", title: "CNPJ", width: "15%", className: "text-align-center" },
        { data: "Cidade", title: "Cidade", width: "15%", className: "text-align-center" },
        { data: "UF", title: "UF", width: "15%", className: "text-align-center" },
    ];

    return new BasicDataTable(idGrid, header, null);
}


function loadGridFiliais() {
    _gridFiliais = gridBasicFiliais(_avisoPeriodico.Filiais.idGrid);
    _gridFiliais.CarregarGrid([]);
}
function recarregarGridFiliais(idGrid) {
    let listaFiliais = _avisoPeriodico.Filiais.val() || [];
    _gridFiliais.CarregarGrid(listaFiliais);
}

function detalhesRegistroClick(e) {
    if (!_gridAvisoPeriodico.ObterMultiplosSelecionados().length > 1) {
        exibirMensagem(tipoMensagem.aviso, "Atenção", "Selecionar somente um registro para visualizar os detalhes!");
    }
    else {
        _avisoPeriodico.Codigo.val(e.Codigo);
        BuscarPorCodigo(_avisoPeriodico, "AvisoPeriodico/BuscarPorCodigo", (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Message);

            recarregarGridFiliais();

        });

        Global.abrirModal('divModalDetalhesAvisoPeriodico');
    }
}

function confirmarAvisoClick() {
    var codigos = obterCodigosRegistrosSelecionados().length == 0 ? [_avisoPeriodico.Codigo.val()] : obterCodigosRegistrosSelecionados();
    var dados = { Codigos: JSON.stringify(codigos) };
    executarReST("AvisoPeriodico/ConfirmarAviso", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                _gridAvisoPeriodico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })
}
function rejeitarAvisoClick() {
    Global.abrirModal("divModalRejeitarAviso");
}

function enviarRejeitarAvisoClick() {
    var codigos = obterCodigosRegistrosSelecionados().length == 0 ? [_avisoPeriodico.Codigo.val()] : obterCodigosRegistrosSelecionados();
    var dados = { Codigos: JSON.stringify(codigos), Justificativa: _modalRejeitarAviso.Justificativa.val() };
    executarReST("AvisoPeriodico/RejeitarAviso", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalRejeitarAviso');
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                _gridAvisoPeriodico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
        Global.fecharModal('divModalRejeitarAviso')
        LimparCampos(_modalRejeitarAviso);
    })
}
function obterCodigosRegistrosSelecionados() {
    var dados = _gridAvisoPeriodico.ObterMultiplosSelecionados();
    var codigos = [];

    for (let i = 0; i < dados.length; i++)
        codigos.push(dados[i].Codigo);

    return codigos;
}


function exportarResumoClick() {
    if (_avisoPeriodico.Codigo.val())
        executarDownload("AvisoPeriodico/ExportarResumo", { Codigo: _avisoPeriodico.Codigo.val() });
    else
        exibirMensagem(tipoMensagem.falha, "Necessário selecionar um registro!")
}
    //#endregion

function verificarCallBack() {
    existeRegistroSelecionado = _gridAvisoPeriodico.ObterMultiplosSelecionados().length > 0

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && existeRegistroSelecionado) {
        _pesquisaAvisoPeriodico.Confirmar.visible(true);
        _pesquisaAvisoPeriodico.Rejeitar.visible(true);

    } else {
        _pesquisaAvisoPeriodico.Confirmar.visible(false);
        _pesquisaAvisoPeriodico.Rejeitar.visible(false);
    }

}