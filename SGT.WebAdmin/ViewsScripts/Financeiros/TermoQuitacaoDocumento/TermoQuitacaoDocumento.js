/// <reference path="../../Enumeradores/EmumAprovacaoPendente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

//#region Variaveis Globais
var _termoQuitacao;
var _pesquitaTermoQuitacao;
var _gridTermoQuitacao;
var _gridFiliais;
var _crudTermoQuitacao;
//#endregion


//#region Constructores

var _statusAprovacao = {
    Todos: "",
    Rejeitados: 3,
    Aprovados: 2,
    AgAprovacao: 1
}
var _statusAprovacaoOpcoes = [
    { text: "Aprovado", value: _statusAprovacao.Aprovados },
    { text: "Rejeitado", value: _statusAprovacao.Rejeitados },
    { text: "Ag. Aprovação", value: _statusAprovacao.AgAprovacao },
    { text: "Todos", value: _statusAprovacao.Todos },
]

function PesquisaTermoQuitacaoFinanceiro() {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.NumeroDeTermo = PropertyEntity({ text: "Número do Termo: ", val: ko.observable(""), getType: typesKnockout.int});
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataVigenciaInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataVigenciaFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.SituacaoAprovacao = PropertyEntity({ text: "Situação Aprovação: ", val: ko.observable(_statusAprovacao.Todos), options: _statusAprovacaoOpcoes, def: _statusAprovacao.Todos });
    this.SituacaoTermoQuitacao = PropertyEntity({ text: "Situação Termo Quitação: ", val: ko.observable(EnumSituacaoTermoQuitacaoFinanceiro.Todas), options: EnumSituacaoTermoQuitacaoFinanceiro.obterOpcoesTransportadorPesquisa(), def: EnumSituacaoTermoQuitacaoFinanceiro.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTermoQuitacao.CarregarGrid();
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
}

function TermoQuitacao() {
    this.NumeroTermo = PropertyEntity({ text: "Número Termo: ", val: ko.observable(0) })
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoProvisao) })
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PagamentosEDescontosViaCreditoEmConta = PropertyEntity({ text: "Pagamentos e descontos via crédito em conta: ", val: ko.observable("R$ 0,00") });
    this.PagamentosEDescontosViaConfiming = PropertyEntity({ text: "Pagamentos e descontos via confirming: ", val: ko.observable("R$ 0,00") });
    this.CreditoEmConta = PropertyEntity({ text: "Crédito em Conta: ", val: ko.observable("R$ 0,00") });
    this.TotalAdiantamento = PropertyEntity({ text: "Total Adiantamento: ", val: ko.observable("R$ 0,00") });
    this.NotasCompensadasAdiantamentos = PropertyEntity({ text: "Notas Compensadas Contra Adiantamentos: ", val: ko.observable("R$ 0,00") });
    this.SaldoAdiantamentoEmAberto = PropertyEntity({ text: "Saldo do adiantamento em aberto: ", val: ko.observable("R$ 0,00") });
    this.TotalGeralPagamento = PropertyEntity({ text: "Total Geral dos Pagamentos: ", val: ko.observable("R$ 0,00") });
    this.PossuiRegrasAprovacao = PropertyEntity({ val: ko.observable(false) });
    this.ExportarResumo = PropertyEntity({ text: 'Exportar Resumo', eventClick: exportarResumoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });

    this.Avancar = PropertyEntity({ text: 'Avançar', eventClick: exportarResumoClick, type: types.event, idGrid: guid(), visible: ko.observable(false) });
}
function CRUDTermoQuitacaoDocumento() {
    this.RejeitarTermo = PropertyEntity({ text: 'Rejeitar Termo', eventClick: rejeitarTermoClick, type: types.event, idGrid: guid(), visible: ko.observable(false) });
    this.AprovarTermo = PropertyEntity({ text: 'Aprovar Termo', eventClick: aprovarTermoClick, type: types.event, idGrid: guid(), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: 'Cancelar', eventClick: cancelarTermoQuitacao, type: types.event, idGrid: guid(), visible: ko.observable(true) });
}

function RejeitarTermo() {
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", val: ko.observable(""), required: ko.observable(true), maxlength: 1000 });
    this.Enviar = PropertyEntity({ text: 'Enviar', eventClick: enviarRejeitarTermoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });

    this.Justificativa.val.subscribe(function (novoValor) {
        $(`#${_modalRejeitarTermo.Justificativa.id}`).removeClass("is-invalid")
    });
}
//#endregion


//#region Funções de Carregamento
function loadTermoQuitacaoDocumento() {
    _pesquitaTermoQuitacao = new PesquisaTermoQuitacaoFinanceiro();
    KoBindings(_pesquitaTermoQuitacao, "knockoutPesquisaTermoQuitacaoFinanceiro");

    _termoQuitacao = new TermoQuitacao();
    KoBindings(_termoQuitacao, "tabDetalhes");

    _crudTermoQuitacaoDocumento = new CRUDTermoQuitacaoDocumento();
    KoBindings(_crudTermoQuitacaoDocumento, "knoutCRUDTermoQuitacaoDocumento");

    _modalRejeitarTermo = new RejeitarTermo();
    KoBindings(_modalRejeitarTermo, "knoutModalRejeitarTermo");

    new BuscarTransportadores(_pesquitaTermoQuitacao.Transportador);

    loadGridTermoQuitacaoFinanceiro();
    loadGridFiliais();
    loadTermoQuitacaoDocumentoPDF();
    loadAnexosDocumentoAssinado();
}
//#endregion


//#region Funções Auxiliares


function loadGridTermoQuitacaoFinanceiro() {
    var editarRegistro = {
        descricao: "Editar",
        id: "clasEditar",
        evento: "onclick",
        metodo: editarRegistroClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editarRegistro]
    }
    var configuracoesExportacao = { url: "TermoQuitacaoFinanceiro/ExportarPesquisa", titulo: "Termo Quitação" };

    _gridTermoQuitacao = new GridViewExportacao(_pesquitaTermoQuitacao.Pesquisar.idGrid, "TermoQuitacaoFinanceiro/Pesquisa", _pesquitaTermoQuitacao, menuOpcoes, configuracoesExportacao);
    _gridTermoQuitacao.CarregarGrid();
}

function exportarResumoClick() {
    console.log("Não desenvolvido")
}

function loadGridFiliais() {
    _gridFiliais = CriGridBasicFiliais(_termoQuitacao.Filiais.idGrid);
    _gridFiliais.CarregarGrid([]);
}
function CriGridBasicFiliais(idGrid) {

    var header = [
        { data: "CodigoIntegracao", title: "Código Integração", width: "15%", className: "text-align-center" },
        { data: "CNPJ", title: "CNPJ", width: "15%", className: "text-align-center" },
        { data: "Cidade", title: "Cidade", width: "15%", className: "text-align-center" },
        { data: "UF", title: "UF", width: "15%", className: "text-align-center" },
    ];

    return new BasicDataTable(idGrid, header, null);
}


function recarregarGridFiliais() {
    let listaFiliais = _termoQuitacao.Filiais.val() || [];
    _gridFiliais.CarregarGrid(listaFiliais);
}

function editarRegistroClick(e) {
    _termoQuitacao.Codigo.val(e.Codigo);
    BuscarPorCodigo(_termoQuitacao, "TermoQuitacaoFinanceiro/BuscarPorCodigo", (arg) => {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Message);

        ControleVisualizacaoCampos(arg.Data);

        recarregarGridFiliais();

        BuscarImagemPorCodigo(e.Codigo);

        FadeTermoQuitacaoDetalhes(true);
        _anexosDocumentoAssinado.DocumentosAssinados.val(arg.Data.DocumentosAssinados);

        if (!_termoQuitacao.PossuiRegrasAprovacao.val())
            _crudTermoQuitacao.Reprocessar.visible(true);
    })
}
function cancelarTermoQuitacao() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    LimparCampos(_termoQuitacao);
    _gridFiliais.CarregarGrid([]);
    FadeTermoQuitacaoDetalhes();
}

function FadeTermoQuitacaoDetalhes(fade = false) {
    if (!fade) {
        $("#cardTermo").removeClass("cardTermoShow");
        $("#cardTermo").addClass("cardTermoHide");
        return;
    }
    $("#cardTermo").removeClass("cardTermoHide");
    $("#cardTermo").addClass("cardTermoShow");
}

function aprovarTermoClick(e) {
    if (_termoQuitacao.Situacao.val() != EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O termo não pode ser rejeitado na situação atual", 20000);
        return;
    }

    var dados = { Codigo: _termoQuitacao.Codigo.val() };
    executarReST("TermoQuitacaoDocumento/AprovarTermo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                _gridTermoQuitacao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })
}
function rejeitarTermoClick(e) {
    if (_termoQuitacao.Situacao.val() != EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O termo não pode ser rejeitado na situação atual", 20000);
        return;
    }

    Global.abrirModal("divModalRejeitarTermo");
}

function enviarRejeitarTermoClick() {
    if (_modalRejeitarTermo.Justificativa.val().length < 160) {
        $(`#${_modalRejeitarTermo.Justificativa.id}`).addClass("is-invalid")
        exibirMensagem(tipoMensagem.aviso, "Dados inválidos", "A justificativa deve conter no mínimo 160 caracteres");
        return;
    }

    var dados = { Codigo: _termoQuitacao.Codigo.val(), Justificativa: _modalRejeitarTermo.Justificativa.val() };
    executarReST("TermoQuitacaoDocumento/RejeitarTermo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal('divModalRejeitarTermo');
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                _gridTermoQuitacao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })

}
function ControleVisualizacaoCampos(termo) {
    _crudTermoQuitacaoDocumento.AprovarTermo.visible(termo.Situacao == EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador || termo.Situacao == EnumSituacaoTermoQuitacao.AguardandoAprovacaoTransportador);
    _crudTermoQuitacaoDocumento.RejeitarTermo.visible(termo.Situacao == EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador || termo.Situacao == EnumSituacaoTermoQuitacao.AguardandoAprovacaoTransportador);
}

//#endregion
