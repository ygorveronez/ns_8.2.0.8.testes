/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoGestaoDevolucao.js" />

//#region Objetos Globais do Arquivo
var _aprovacaoTipoDevolucao;
var _gridVisualizacaoProdutos;
var _gridVisualizacaoNotasFiscaisOrigem;
// #endregion Objetos Globais do Arquivo

//#region Classes
var AprovacaoTipoDevolucao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.GridProdutos = PropertyEntity({ id: guid() });
    this.GridNotasFiscais = PropertyEntity({ id: guid() });
    this.NumerosNotasFiscais = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Números NF-es", visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500, enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(""), text: "Tipo Devolução", visible: ko.observable(true) });
    this.AlterarTipoDevolucao = PropertyEntity({ visible: ko.observable(true), text: "Alterar Tipo de Devolução", val: ko.observable(EnumTipoGestaoDevolucao.Selecione), options: ko.observable(EnumTipoGestaoDevolucao.obterOpcaoDescarte()), def: EnumTipoGestaoDevolucao.Selecione, enable: ko.observable(true) });
    this.VisualizarProdutos = PropertyEntity({ type: types.event, eventClick: null, text: "Visualizar produtos", idGrid: guid(), visible: ko.observable(true) });
    this.VisualizarNotasFiscais = PropertyEntity({ type: types.event, eventClick: null, text: "Visualizar notas", idGrid: guid(), visible: ko.observable(true) });

    this.Aprovar = PropertyEntity({
        type: types.event, eventClick: aprovarTipoDevolucao, text: "Aprovar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.Reprovar = PropertyEntity({
        type: types.event, eventClick: reprovarTipoDevolucao, text: "Reprovar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadAprovacaoTipoDevolucao(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/AprovacaoTipoDevolucao.html?dyn=" + guid(), function (data) {
                $("#container-principal-content").html("");
                $("#container-principal-content").html(data);

                _aprovacaoTipoDevolucao = new AprovacaoTipoDevolucao();
                KoBindings(_aprovacaoTipoDevolucao, "knockoutAprovacaoTipoDevolucao");

                PreencherObjetoKnout(_aprovacaoTipoDevolucao, r);

                controlarOpcoesAlterarTipoDevolucao();
                controlarAcoesContainerPrincipal(etapa, _aprovacaoTipoDevolucao);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function loadGridVisualizacaoProdutos() {
    _gridVisualizacaoProdutos = new GridView(_aprovacaoTipoDevolucao.GridProdutos.id, "GestaoDevolucao/BuscarProdutosNotasFiscais", _aprovacaoTipoDevolucao, null, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridVisualizacaoProdutos.CarregarGrid();
}

function loadGridVisualizacaoNotasFiscaisOrigem() {
    _gridVisualizacaoNotasFiscaisOrigem = new GridView(_aprovacaoTipoDevolucao.GridNotasFiscais.id, "GestaoDevolucao/PesquisaNotasFiscaisOrigem", _aprovacaoTipoDevolucao, null, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridVisualizacaoNotasFiscaisOrigem.CarregarGrid();
}

function aprovarTipoDevolucao() {
    executarReST("GestaoDevolucao/AprovarTipoGestaoDevolucao", { Codigo: _aprovacaoTipoDevolucao.Codigo.val(), Observacao: _aprovacaoTipoDevolucao.Observacao.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Devolução aprovada com sucesso");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function reprovarTipoDevolucao() {

    if (_aprovacaoTipoDevolucao.AlterarTipoDevolucao.val() == EnumTipoGestaoDevolucao.Selecione) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Necessário escolher o tipo da devolução para reprovar.");
        return;
    }

    let dados = {
        Codigo: _aprovacaoTipoDevolucao.Codigo.val(),
        TipoGestaoDevolucao: _aprovacaoTipoDevolucao.AlterarTipoDevolucao.val(),
        Observacao: _aprovacaoTipoDevolucao.Observacao.val()
    };

    executarReST("GestaoDevolucao/ReprovarTipoGestaoDevolucao", dados, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Devolução reprovada com sucesso");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function visualizarProdutos() {
    loadGridVisualizacaoProdutos();
    Global.abrirModal("divModalVisualizarProdutosAprovacaoTipoDevolucao");
}

function visualizarNotasFiscais() {
    loadGridVisualizacaoNotasFiscaisOrigem();
    Global.abrirModal("divModalVisualizarNotasFiscaisAprovacaoTipoDevolucao");
}
// #endregion Funções Públicas

// #region Funções Privadas
function controlarOpcoesAlterarTipoDevolucao() {
    if (_aprovacaoTipoDevolucao.TipoDevolucao.val() == 'Coleta' || _aprovacaoTipoDevolucao.TipoDevolucao.val() == 'Agendamento') {
        _aprovacaoTipoDevolucao.AlterarTipoDevolucao.options(EnumTipoGestaoDevolucao.obterOpcaoDescarteEPermuta());
    }
}
// #endregion Funções Privadas